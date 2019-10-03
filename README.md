# NSDDFeAPIClientJava

## Introdução:

Esta documentação apresenta trechos de códigos de uma classe em C# que foi desenvolvida para consumir as funcionalidades da NS DDF-e API.

-----

## Integrando ao sistema:

Para utilizar as funções de comunicação com a API, você precisa realizar os seguintes passos:

1. Extraia o conteúdo da pasta compactada que você baixou;
2. Copie para sua aplicação a pasta src, na qual contem todos as classes que serão utilizadas;
3. Abra o seu projeto e importe a pasta copiada.
4. A aplicação utiliza o package 'Newtonsoft.Json' para facilitar a deserialização do JSON, respectivamente. 

![dependency](https://confluence.ns.eti.br/download/attachments/25690610/image2019-3-18%208%3A24%3A8.png?version=1&modificationDate=1552908140508&api=v2)

**Pronto!** Agora, você já pode consumir a NS DDF-e API através do seu sistema. Todas as funcionalidades de comunicação foram implementadas na classe DDFeAPI.cs. Caso tenha dúvidas de como adicionar este package, veja o tutorial a seguir: [Package Newtonsoft.Json](https://docs.microsoft.com/pt-br/nuget/consume-packages/install-use-packages-visual-studio#finding-and-installing-a-package).

-----

## Realizando uma Manifestação:

Para realizar uma manifestação de um documento emitido contra o CNPJ do seu cliente ou seu, você poderá utilizar a função manifestacao da classe DDFeAPI. Veja abaixo sobre os parâmetros necessários, e um exemplo de chamada do método.

#### Parâmetros:

ATENÇÃO: o token também é um parâmetro necessário, e você deve primeiramente defini-lo na classe DDFeAPI.cs. Verifique os parâmetros da classe.

![dependency](https://confluence.ns.eti.br/download/attachments/28412620/image2019-10-3%2014%3A27%3A34.png?version=1&modificationDate=1570123656027&api=v2)

Parametros     | Descrição
:-------------:|:-----------
CNPJInteressado | Conteúdo de emissão do documento.
tpEvento        | Tipo de evento posto na manifestação:<ul> <li>**210200** – Confirmação da Operação</li> <li>**210240** – Operação não Realizada</li> <li>**210220** – Desconhecimento da Operação</li> <li>**210210** – Ciência da Operação</li> </ul>
nsu             | Número Sequencial Único de um DF-e determinado
xJust           | Justificativa da manifestação (Informar somente quando o tpEvento for 210240)
chave           | Chave do DF-e que deseja-se manifestar


#### Exemplo de chamada:

Após ter todos os parâmetros listados acima, você deverá fazer a chamada da função. Veja o código de exemplo abaixo:
    
    //Por nsu
    String retorno = DDFeAPI.manifestacao("11111111111111", "210200", "134");
     
    //Por chave
    String retorno = DDFeAPI.manifestacao("11111111111111", "210200", "", "", "35160324110220000136550010000000351895912462");
     
    MessageBox.Show(retorno);


A função manifestacao fará o envio da confirmação de participação do destinatário na operação acobertada pela Nota Fiscal Eletrônica, emitida para o seu CNPJ, para API.

-----

## Realizando um Download Único:

Para realizar um download de um unico documento, você poderá utilizar a função downloadUnico da classe DDFeAPI. Veja abaixo sobre os parâmetros necessários, e um exemplo de chamada do método.

#### Parâmetros:

ATENÇÃO: o token também é um parâmetro necessário, e você deve primeiramente defini-lo na classe DDFeAPI.cs. Verifique os parâmetros da classe.

![dependency](https://confluence.ns.eti.br/download/attachments/28412620/image2019-10-3%2014%3A27%3A34.png?version=1&modificationDate=1570123656027&api=v2)

Parametros      | Descrição
:-------------: |:-----------
CNPJInteressado | Conteúdo de emissão do documento.
caminho         | Local onde serão salvos os documentos
tpAmb           | Tipo de ambiente evento posto na manifestação:<ul><li>1 - Produção</li><li>2 - Homologação</li></ul>
nsu             | Número Sequencial Único do DF-e que deseja-se fazer o downloadmodelo
modelo          | Modelo do documento:<ul> <li>55 - NF-e</li> <li>98 - NFSe SP</li> <li>57 - CT-e</li> </ul>
chave           | Chave do DF-e que deseja-se manifestar
incluirPdf      | Incluir do documento auxiliar
apenasComXml    | Carregar apenas documentos com XMLs disponíveis
comEventos      | Incluir eventos vinculados ao documento disponíveis

#### Exemplo de chamada:

Após ter todos os parâmetros listados acima, você deverá fazer a chamada da função. Veja o código de exemplo abaixo:


    //Por nsu
    String retorno = DDFeAPI.downloadUnico("11111111111111", "C:\\Notas\\", "2", "134", "55", "", true, true, false);
     
    //Por chave
    String retorno = DDFeAPI.downloadUnico("11111111111111", "C:\\Notas\\", "2", "", "55", "35160324110220000136550010000000351895912462", false, false, true);
     
    MessageBox.Show(retorno);

A função downloadUnico fará o envio de um json para API fazendo com que o documento especifico na requisição seja baixado e salvo na maquina.

-----

## Realizando um Donwload em Lote:

Para realizar um download de lote de documentos, você poderá utilizar a função downloadLote da classe DDFeAPI. Veja abaixo sobre os parâmetros necessários, e um exemplo de chamada do método.

#### Parâmetros:

ATENÇÃO: o token também é um parâmetro necessário, e você deve primeiramente defini-lo na classe DDFeAPI.cs. Verifique os parâmetros da classe.

![dependency](https://confluence.ns.eti.br/download/attachments/28412620/image2019-10-3%2014%3A27%3A34.png?version=1&modificationDate=1570123656027&api=v2)

Parametros      | Descrição
:-------------: |:-----------
CNPJInteressado | Conteúdo de emissão do documento.
caminho         | Local onde serão salvos os documentos
tpAmb           | Tipo de ambiente evento posto na manifestação:<ul><li>1 - Produção</li><li>2 - Homologação</li></ul>
ultNSU          | Ultimo Número Sequencial Único para fazer download a partir do mesmo
modelo          | Modelo do documento:<ul> <li>55 - NF-e</li> <li>98 - NFSe SP</li> <li>57 - CT-e</li> </ul>
apenasPendManif | Carregar apenas documentos pendentes de manifestação
incluirPdf      | Incluir do documento auxiliar
apenasComXml    | Carregar apenas documentos com XMLs disponíveis
comEventos      | Incluir eventos vinculados ao documento disponíveis

#### Exemplo de chamada:

Após ter todos os parâmetros listados acima, você deverá fazer a chamada da função. Veja o código de exemplo abaixo:

    String retorno = DDFeAPI.downloadLote("11111111111111", "C:\\Notas\\", "2", "0", "55", false, false, true, true);
    MessageBox.Show(retorno);
    
A função downloadLote fará o envio de um json para API fazendo com que os documentos, a partir do ultimo NSU, sejam baixados e salvos na maquina.

-----

![Ns](https://nstecnologia.com.br/blog/wp-content/uploads/2018/11/ns%C2%B4tecnologia.png) | Obrigado pela atenção!
