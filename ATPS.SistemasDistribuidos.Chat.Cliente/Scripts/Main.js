var moduloChat = angular.module("ModuloChat", []);

var urlWs = 'ws://localhost:6100/';
var urlWsHttp = 'http://localhost:6100/';
//var urlWs = "ws://atpssdws.azurewebsites.net/api/";


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

