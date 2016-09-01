var moduloChat = angular.module("ModuloChat", []);

moduloChat.factory('$webSocket', function () {
    var webSockets;

    var Conectar = function (remetente) {
        if (!webSockets || webSockets.readyState != WebSocket.OPEN) {
            CriarWebSockets(remetente);
        } else {
            webSockets.close();
        }
    }

    var OnOpen = function (onopen) {
        webSockets.onopen = onopen;
    };

    var OnError = function () {
        webSockets.onerror = function (e) {
            alert(e);
        }
    }

    var OnMessage = function (onMessage) {
        webSockets.onmessage = onMessage;
    }

    var OnClose = function myfunction(onClose) {
        webSockets.onclose = onClose;
    };

    var Enviar = function (destinatario,  mensagem) {

        if (ConexaoAberta()) {
            webSockets.send("{Conversa:{Destinatario:{Nome:'" + destinatario
                + "'}}, Texto:'" + mensagem + "'}");
        }
    }

    function ConexaoAberta() {
        return webSockets.readyState == WebSocket.OPEN;
    }

    function CriarWebSockets(remetente) {
        if (!remetente) {
            alert("Favor informar o Remetente e o Destinatário");
            return;
        }

        var raizAplicacao = "ws://localhost:6100/api/";
        webSockets = new WebSocket(raizAplicacao + "Chat?Remetente=" + remetente);
    }

    return {
        Conectar: Conectar,
        OnOpen: OnOpen,
        OnError: OnError,
        OnMessage: OnMessage,
        OnClose: OnClose,
        Enviar: Enviar
    }
});

moduloChat.controller('ChatController', function ($scope, $http, $webSocket) {

    $scope.Chat = {};
    $scope.Chat.Conversas = [/*{ Destinatario: { Nome: "Marcos" }, Remetente: { Nome: "Nome" } }*/];
    $scope.Chat.Remetente = {};
    $scope.Chat.Destinatario = {};
    $scope.Chat.MensagensDestinatario = [];
    $scope.Chat.Status = "Desconectado";
    $scope.Chat.MensagemStatus = "Conectar";

    $scope.Chat.ConectarDesconectar = function (conversa) {

        $webSocket.Conectar($scope.Chat.Remetente.Nome);

        $webSocket.OnMessage(function (mensagem) {
            var retorno = JSON.parse(mensagem.data);
            console.log(retorno);
            if ($scope.Chat.Conversas.length == 0) {
                Enumerable.From(retorno.Conversas).ForEach(function (conversa) {
                    $scope.Chat.Conversas.push(conversa);
                    $scope.$apply();
                });
            } else {
                Enumerable.From(retorno.Conversas).ForEach(function (conversa) {
                    var conversaExistente = Enumerable.From($scope.Chat.Conversas).FirstOrDefault(null, function (item) {
                        return conversa.Destinatario.Nome == item.Destinatario.Nome && conversa.Remetente.Nome == item.Remetente.Nome
                    });

                    if (conversaExistente) {
                        conversaExistente.Mensagens = conversa.Mensagens;
                        if (conversa.Destinatario.Nome == $scope.Chat.Destinatario.Nome && conversa.Remetente.Nome == $scope.Chat.Remetente.Nome) {
                            $scope.Chat.MensagensDestinatario = conversa.Mensagens;
                            $scope.$apply();
                        }
                    } else {
                        $scope.Chat.Conversas.push(conversa);
                        $scope.$apply();
                    }
                });
            }
        });

        $webSocket.OnOpen(function () {
            $scope.Chat.Status = "Conectado";
            $scope.Chat.Conectado = true;
            $scope.Chat.MensagemStatus = "Desconectar";
            $scope.$apply();
        });

        $webSocket.OnError(function (e) {
            alert(e.data);
        });
    };

    $scope.Chat.SelecionarDestinatario = function (conversa) {
        Enumerable.From($scope.Chat.Conversas).Where(function (con) {
            return con.Selecionada;
        }).ForEach(function (con) {
            con.Selecionada = false;
        });

        conversa.Selecionada = true;
        $scope.Chat.Conversa = conversa;
        $scope.Chat.Destinatario = conversa.Destinatario;
        $scope.Chat.MensagensDestinatario = conversa.Mensagens;

    };

    $scope.Chat.Remetente.Enviar = function () {
        if (!$scope.Chat.Remetente.Nome || !$scope.Chat.Destinatario.Nome) {
            alert("Favor informar o Remetente e o Destinatário");
            return;
        }
        if ($scope.Chat.Destinatario.Mensagem) {
            $webSocket.Enviar($scope.Chat.Destinatario.Nome, $scope.Chat.Destinatario.Mensagem);
        }
    };
})

