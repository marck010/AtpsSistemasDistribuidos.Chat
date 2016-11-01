var moduloChat = angular.module("ModuloChat", []);

var urlWsHttp = "http://localhost:6100/";//"http://aptssd.ddns.net/SacWs";
var urlWs = "ws://localhost:6100/";//"ws://aptssd.ddns.net/SacWs";


function TratarErro(retorno, matarSessao) {

    if (retorno.TipoErro == 1) {
        matarSessao();
    }
    if (retorno.TipoErro == 3) {
        alert(retorno.Error);
    }

    if (retorno.TipoErro == 2) {
        alert("Ocorreu um erro inesperado");
    }

    matarSessao();

}

