var moduloChat = angular.module("ModuloChat", []);

var urlwshttp = "http://146.148.78.240/";
var urlws = "ws://146.148.78.240/";
//var urlWsHttp = "http://localhost:6100/";
//var urlWs = "ws://localhost:6100/";


function TratarErro(retorno, matarSessao) {

    if (retorno.TipoErro == 1) {
        matarSessao();
    }

    else if (retorno.TipoErro == 3) {
        alert(retorno.Error);
    }

    else if (retorno.TipoErro == 2) {
        alert("Ocorreu um erro inesperado");
    }
    else {
        alert("Ocorreu um erro inesperado");
    }

    if (typeof (matarSessao) == "function") {
        matarSessao();
    }

    console.log(retorno);
}

