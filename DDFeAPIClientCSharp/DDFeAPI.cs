using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DDFeAPIClientCSharp
{
    public class DDFeAPI
    {   
        private static String token = "COLOQUE_TOKEN";

        // Esta função envia um conteúdo para uma URL, em requisições do tipo POST
        private static String enviaConteudoParaAPI(String conteudo, String url)
        {
            String retorno = "";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers["X-AUTH-TOKEN"] = token;
            httpWebRequest.ContentType = "application/json;charset=utf-8";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(conteudo);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                // Faz o envio por POST do json para a url
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) retorno = streamReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = (HttpWebResponse)ex.Response;
                    using (var streamReader = new StreamReader(response.GetResponseStream())) retorno = streamReader.ReadToEnd();
                    switch ((int)response.StatusCode)
                    {
                        //Se o token não for enviado ou for inválido
                        case 401:
                            MessageBox.Show("Token não enviado ou inválido");
                            break;
                        //Se o token informado for inválido 403
                        case 403:
                            MessageBox.Show("Token sem permissão");
                            break;
                        //Se não encontrar o que foi requisitado
                        case 404:
                            MessageBox.Show("Não encontrado, verifique o retorno para mais informações");
                            break;
                        //Caso contrário
                        default:
                            break;
                    }
                }

            }
            // Devolve o json de retorno da API
            return retorno;
        }



        // Faz a requisição de manifestação para API
        public static String manifestacao(String CNPJInteressado, String tpEvento, String nsu, String xJust = "", String chave = "")
        {

            ManifestacaoJSON parametros = new ManifestacaoJSON();
            parametros.CNPJInteressado = CNPJInteressado;

            if (String.IsNullOrEmpty(nsu))
            {
                parametros.chave = chave;
            } 
            else
            {
                parametros.nsu = nsu;
            }

            parametros.manifestacao.tpEvento = tpEvento;
            
            if (tpEvento.Equals("210240"))
            {
                parametros.manifestacao.xJust = xJust;
            }


            String json = JsonConvert.SerializeObject(parametros, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            String url = "https://ddfe.ns.eti.br/events/manif";

            gravaLinhaLog("[MANIFESTACAO_DADOS]");
            gravaLinhaLog(json);

            String resposta = enviaConteudoParaAPI(json, url);

            gravaLinhaLog("[MANIFESTACAO_RESPOSTA]");
            gravaLinhaLog(json);

            tratamentoManifestacao(resposta);

            return resposta;       
        }

        // Trata o retorno da manifestação da API
        private static void tratamentoManifestacao(String jsonRetorno)
        {
            String xMotivo;

            dynamic respostaJSON = JsonConvert.DeserializeObject(jsonRetorno);
            String status = respostaJSON.status;

            if (status.Equals("200"))
            {
                xMotivo = respostaJSON.retEventoxMotivo;
            }
            else if (status.Equals("-3"))
            {
                xMotivo = respostaJSON.erro.xMotivo;
            }
            else
            {
                xMotivo = respostaJSON.motivo;
            }

            MessageBox.Show(xMotivo);
        }



        // Faz a requisição de download de um unico documento 
        public static String downloadUnico(String CNPJInteressado, String caminho, String tpAmb, String nsu, String modelo,
                                       String chave, Boolean incluirPdf, Boolean apenasComXml, Boolean comEventos)
        {
            // Preenche o objeto da classe e transforma em JSON
            DownloadUnicoJSON parametros = new DownloadUnicoJSON();
            parametros.CNPJInteressado = CNPJInteressado;
            parametros.tpAmb = tpAmb;
            parametros.incluirPDF = incluirPdf;

            if (String.IsNullOrEmpty(nsu))
            {
                parametros.chave = chave;
                parametros.apenasComXml = apenasComXml;
                parametros.comEventos = comEventos;
            }
            else
            {
                parametros.nsu = nsu;
                parametros.modelo = modelo;
            }

            String json = JsonConvert.SerializeObject(parametros, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            String url = "https://ddfe.ns.eti.br/dfe/unique";

            gravaLinhaLog("[DOWNLOAD_UNICO_DADOS]");
            gravaLinhaLog(json);

            String resposta = enviaConteudoParaAPI(json, url);

            gravaLinhaLog("[DOWNLOAD_UNICO_RESPOSTA]");
            gravaLinhaLog(resposta);

            salvaDocs(caminho, incluirPdf, resposta);

            return resposta;
        }


        
        //Faz a requisição de download de um lote de documentos
        public static String downloadLote(String CNPJInteressado, String caminho, String tpAmb, int ultNSU, String modelo,
                                      Boolean apenasPendManif, Boolean incluirPdf, Boolean apenasComXml, Boolean comEventos)
        {
            DownloadLoteJSON parametros = new DownloadLoteJSON();
            parametros.CNPJInteressado = CNPJInteressado;
            parametros.ultNSU = ultNSU;
            parametros.modelo = modelo;
            parametros.tpAmb = tpAmb;
            parametros.incluirPDF = incluirPdf;

            if (!apenasPendManif)
            {
                parametros.apenasComXml = apenasComXml;
                parametros.comEventos = comEventos;
            }
            else
            {
                parametros.apenasPendManif = true;
            }
            String json = JsonConvert.SerializeObject(parametros, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            
            String url = "https://ddfe.ns.eti.br/dfe/bunch";

            gravaLinhaLog("[DOWNLOAD_LOTE_DADOS]");
            gravaLinhaLog(json);

            String resposta = enviaConteudoParaAPI(json, url);

            gravaLinhaLog("[DOWNLOAD_LOTE_RESPOSTA]");
            gravaLinhaLog(resposta);

            salvaDocs(caminho, incluirPdf, resposta);

            return resposta;

        }



        // Realiza o salvamento de documentos feito nas requisições de download da API
        private static void salvaDocs(String caminho, Boolean incluirPdf, String jsonRetorno)
        {
            String resposta;
            String xml;
            String chave;
            String modelo;
            String pdf;
            String tpEvento;
            Boolean listaDocs;

            dynamic respostaJSON = JsonConvert.DeserializeObject(jsonRetorno);
            String status = respostaJSON.status;


            if (status.Equals("200"))
            {
       
                try
                {
                    listaDocs = respostaJSON.listaDocs;
                }
                catch
                {
                    listaDocs = true;
                }
                
                if (!listaDocs)
                {
                    xml = respostaJSON.xml;
                    chave = respostaJSON.chave;
                    modelo = respostaJSON.modelo;
                    salvarXML(xml, caminho, chave, modelo, "");
                    if (incluirPdf)
                    {
                        pdf = respostaJSON.pdf;
                        salvarPDF(pdf, caminho, chave, modelo, "");
                    }
                }
                else
                {
                    dynamic arrayDocs = respostaJSON.xmls;
                    foreach (dynamic itemDoc in arrayDocs)
                    {
                        xml = itemDoc.xml;

                        if (!String.IsNullOrEmpty(xml))
                        {
                            chave = itemDoc.chave;
                            modelo = itemDoc.modelo;
                            tpEvento = itemDoc.tpEvento;
                            if (!String.IsNullOrEmpty(tpEvento))
                            {
                                salvarXML(xml, caminho, chave, modelo, tpEvento);
                            }
                            else
                            {
                                salvarXML(xml, caminho, chave, modelo);
                            }

                            if (incluirPdf)
                            {
                                pdf = itemDoc.pdf;
                                salvarPDF(pdf, caminho, chave, modelo, tpEvento);
                            }
                        }
                    }
                }

                String ultNSU = respostaJSON.ultNSU;
                if (String.IsNullOrEmpty(ultNSU))
                {
                    resposta = "Donwload Unico feito com sucesso!!!";
                }
                else
                {
                    resposta = "Ultimo NSU:" + ultNSU;
                }

            }
            else
            {
                resposta = respostaJSON.motivo;
            }

            MessageBox.Show(resposta);
        }

        // Esta função salva um XML
        private static void salvarXML(String xml, String caminho, String chave, String modelo, String tpEvento = "")
        {
            String extensao;
            if (modelo.Equals("55"))
            {
                extensao = "-procNFe.xml";
            }
            else if (modelo.Equals("57"))
            {
                extensao = "-procCTe.xml";
            }
            else
            {
                extensao = "-procNFSeSP.xml";
            }

            String path = caminho + "\\xmls\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            String localParaSalvar = path + tpEvento + chave + extensao;

            String ConteudoSalvar = xml.Replace(@"\", @"");
            File.WriteAllText(localParaSalvar, ConteudoSalvar);
        }

        // Esta função salva um PDF
        private static void salvarPDF(String pdf, String caminho, String chave, String modelo, String tpEvento = "")
        {
            String extensao;
            if (modelo.Equals("55"))
            {
                extensao = "-procNFe.pdf";
            }
            else if (modelo.Equals("57"))
            {
                extensao = "-procCTe.pdf";
            }
            else
            {
                extensao = "-procNFSeSP.pdf";
            }
            String path = caminho + "\\pdfs\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            String localParaSalvar = path + tpEvento + chave + extensao;

            byte[] bytes = Convert.FromBase64String(pdf);
            if (File.Exists(localParaSalvar)) File.Delete(localParaSalvar);
            FileStream stream = new FileStream(localParaSalvar, FileMode.CreateNew);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();
        }

        // Esta função grava uma linha de texto em um arquivo de log
        private static void gravaLinhaLog(String conteudo)
        {
            String caminho = Application.StartupPath + "\\log\\";

            Console.Write(caminho);
            if (!Directory.Exists(caminho))
            {
                Directory.CreateDirectory(caminho);
            }
            String data = DateTime.Now.ToShortDateString();
            String hora = DateTime.Now.ToShortTimeString();
            String nomeArq = data.Replace("/", "");

            using (StreamWriter outputFile = new StreamWriter(caminho + nomeArq + ".txt", true))
            {
                outputFile.WriteLine(data + " " + hora + " - " + conteudo);
            }
        }


        //Classes utilizadas para geração dos JSON utilizados nos métodos

        //Manifestação
        public class ManifestacaoJSON
        {
            public String CNPJInteressado = null;
            public String nsu = null;
            public String chave = null;
            public Manifestacao manifestacao;
        }
        public class Manifestacao
        {
            public String tpEvento = null;
            public String xJust = null;
        }

        //Download Unico
        public class DownloadUnicoJSON
        {
            public String CNPJInteressado = null;
            public String nsu = null;
            public String chave = null;
            public String modelo = null;
            public String tpAmb = null;
            public Boolean apenasComXml = false;
            public Boolean incluirPDF = false;
            public Boolean comEventos = false;

        }

        //Download Lote
        public class DownloadLoteJSON
        {
            public String CNPJInteressado = null;
            public int ultNSU;
            public String modelo = null;
            public String tpAmb = null;
            public Boolean apenasPendManif = false;
            public Boolean apenasComXml = false;
            public Boolean incluirPDF = false;
            public Boolean comEventos = false;
        }

    }
}