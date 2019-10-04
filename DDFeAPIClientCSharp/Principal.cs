using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDFeAPIClientCSharp
{
    class Principal
    {
        static void Main(string[] args)
        {
            /*
              Aqui voce pode testar a chamada de metodos para manifestar,
              fazer o download de um unico documento ou/e fazer download de
              varios documentos emitidos contra co CNPJ do cliente

              - Aqui um exemplo de chamada de download de um unico documento atraves da chave
                (pode ser feito tanto pela chave do documento ou pelo NSU do mesmo):

                  * String resposta = DDFeAPI.downloadUnico(CNPJInteressado, caminho, tpAmb, nsu, modelo, chave,
                                      incluirPdf, apenasComXml, comEventos);


              - Aqui um exemplo de chamada de download de lote de documentos
                (somente pode ser feito pelo ultimo NSU):

                  * String resposta = DDFeAPI.downloadLote(CNPJInteressado, caminho, tpAmb, ultNSU, modelo,
                                        apenasPendManif, incluirPdf, apenasComXml, comEventos);

              Para maiores informações, consulte a documentação no link: https://confluence.ns.eti.br/display/PUB/C%23+-+DDF-e+API, e entre em contato com a equipe de integração
           */
        }
    }
}
