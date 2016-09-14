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

    var Enviar = function (destinatario, mensagem, identificador) {

        if (ConexaoAberta()) {
            webSockets.send(" {Conversa:{Identificador:'" + identificador + "'}, Destinatario:{Nome:'" + destinatario + "'}, Texto:'" + mensagem + "'}");
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
    $scope.Chat.Usuarios = [/*{ Destinatario: { Nome: "Marcos" }, Remetente: { Nome: "Nome" } }*/];
    $scope.Chat.Remetente = {};
    $scope.Chat.Destinatario = {};
    $scope.Chat.Conversa = {};
    $scope.Chat.Conversa.Mensagens = [];
    $scope.Chat.Status = "Desconectado";
    $scope.Chat.MensagemStatus = "Conectar";

    $scope.Chat.ConectarDesconectar = function () {

        $webSocket.Conectar($scope.Chat.Remetente.Nome);

        $webSocket.OnMessage(function (mensagem) {
            var retorno = JSON.parse(mensagem.data);
            console.log(retorno);
            if ($scope.Chat.Usuarios.length == 0) {
                $scope.Chat.Usuarios.push(retorno);
                $scope.$apply();
            }
            else {
                var usuarioExistente = Enumerable.From($scope.Chat.Usuarios).FirstOrDefault(null, function (usuarioRemetente) {
                    return retorno.Usuario.Nome == usuarioRemetente.Usuario.Nome
                });

                if (usuarioExistente) {
                    if (retorno.Conversa) {
                        usuarioExistente.Conversa = retorno.Conversa;
                        if (usuarioExistente.Selecionado) {
                            $scope.Chat.Conversa = usuarioExistente.Conversa;
                            $scope.Chat.MensagensDestinatario = usuarioExistente.Conversa.Mensagens;
                            $scope.$apply();
                        }
                    }

                } else {
                    $scope.Chat.Usuarios.push(retorno);
                    $scope.$apply();
                }
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

    $scope.Chat.SelecionarDestinatario = function (usuarioSelecionado) {

        Enumerable.From($scope.Chat.Usuarios).Where(function (usuario) {
            return !!usuario.Selecionado;
        }).ForEach(function (usuario) {
            usuario.Selecionado = false;
        });

        usuarioSelecionado.Selecionado = true;
        $scope.Chat.Conversa = usuarioSelecionado.Conversa;
        $scope.Chat.Destinatario = usuarioSelecionado.Usuario;

    };

    $scope.Chat.Remetente.Enviar = function () {
        if (!$scope.Chat.Remetente.Nome || !$scope.Chat.Destinatario.Nome) {
            alert("Favor informar o Remetente e o Destinatário");
            return;
        }
        if ($scope.Chat.Destinatario.Mensagem) {
            var identificador = !$scope.Chat.Conversa || !$scope.Chat.Conversa.Identificador ? '' : $scope.Chat.Conversa.Identificador;
            $webSocket.Enviar($scope.Chat.Destinatario.Nome, $scope.Chat.Destinatario.Mensagem, identificador);
            $scope.Chat.Conversa.Mensagens.push({ Texto: $scope.Chat.Destinatario.Mensagem, Remetente: $scope.Chat.Remetente });
            $scope.Chat.Destinatario.Mensagem = '';
        }
    };
})

