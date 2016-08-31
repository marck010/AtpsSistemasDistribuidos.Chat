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

    var Enviar = function (destinatario, mensagem) {
        webSockets.upgradeReq.url + "&destinatario=" + destinatario;

        if (ConexaoAberta()) {
            webSockets.send(mensagem);
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
    $scope.Chat.Conversas = [{ Destinatario: { Nome: "Marcos" }, Remetente: { Remetente: { Nome: "Nome" } } }];
    $scope.Chat.Remetente = {};
    $scope.Chat.Destinatario = {};
    $scope.Chat.MensagensDestinatario = [];
    $scope.Chat.Status = "Desconectado";
    $scope.Chat.MensagemStatus = "Conectar";

    $scope.Chat.ConectarDesconectar = function (conversa) {

        $webSocket.Conectar($scope.Chat.Remetente.Nome);

        $webSocket.OnMessage(function (mensagem) {
            var conversa = JSON.parse(mensagem.data);

            if ($scope.Chat.Conversas.length == 0) {
                $scope.Chat.Conversas.push(conversa);
            } else {

                Enumerable.From($scope.Chat.Conversas).ForEach(function (item) {
                    if (conversa.Destinatario.Nome == item.Destinatario.Nome && conversa.Remetente.Nome == item.Remetente.Nome) {
                        item.Mensagens = conversa.Mensagens;
                        if (conversa.Destinatario.Nome == $scope.Chat.Destinatario.Nome && conversa.Remetente.Nome == $scope.Chat.Remetente.Nome) {
                            $scope.Chat.MensagensDestinatario = conversa.Mensagens;
                        }
                    } else {
                        $scope.Chat.Conversas.push(conversa);
                    }
                });
            }
        });

        $webSocket.OnOpen(function () {
            $scope.Chat.Status = "Conectado";
            $scope.Chat.Conectado = true;
            $scope.Chat.MensagemStatus = "Desconectar";
        });

        $webSocket.OnOpen(function () {
            $scope.Chat.Status = "Conectado";
            $scope.Chat.MensagemStatus = "Desconectar";
        });
    };

    $scope.Chat.SelecionarDestinatario = function (conversa) {
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

