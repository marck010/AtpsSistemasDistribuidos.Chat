var moduloChat = angular.module("ModuloChat", []);

var urlWs = 'ws://localhost:6100/';
var urlWsHttp = 'http://localhost:6100/';
//var urlWs = "ws://atpssdws.azurewebsites.net/api/";


moduloChat.config(function ($httpProvider) {
    $httpProvider.defaults.headers.common = {};
    $httpProvider.defaults.headers.post = {};
    $httpProvider.defaults.headers.put = {};
    $httpProvider.defaults.headers.patch = {};
});
