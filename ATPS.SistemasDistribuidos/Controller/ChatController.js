var moduloChat = angular.module("ModuloChat", []);

moduloChat.controller('ChatController', function ($scope, $http) {

  
    var webSockets;
    $scope.Chat = {};
    $scope.Chat.Conversas = [];
    $scope.Chat.Remetente = {};
    $scope.Chat.Destinatario = {};
    $scope.Chat.MensagensDestinatario = [];
    $scope.Chat.Status = "Desconectado";
    $scope.Chat.MensagemStatus = "Conectar";

    $scope.Chat.ConectarDesconectar = function (conversa) {
        if ($scope.Chat.MensagemStatus = "Conectar") {
            CriarWebSockets();

        } else {
            webSockets.close();
        }
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
        if (webSockets.readyState == WebSocket.OPEN && $scope.Chat.Destinatario.Mensagem) {
            webSockets.send($scope.Chat.Destinatario.Mensagem);
        }
    };

    function CriarWebSockets() {
        if (webSockets) {
            return;
        }

        if (!$scope.Chat.Remetente.Nome || !$scope.Chat.Destinatario.Nome) {
            alert("Favor informar o Remetente e o Destinatário");
            return;
        }

        var raizAplicacao = "ws://localhost:6100/api/";
        webSockets = new WebSocket(raizAplicacao + "Chat?Remetente=" + $scope.Chat.Remetente.Nome + "&Destinatario=" + $scope.Chat.Destinatario.Nome)

        webSockets.onopen = function () {
            $scope.Chat.Status = "Conectado";
            $scope.Chat.MensagemStatus = "Desconectar";
        }

        webSockets.onerror = function (e) {
            alert(e);
        }

        webSockets.onmessage = function (mensagem) {

            var objeto = JSON.parse(mensagem.data);
            
            $scope.Chat.Conversas.push(objeto);
        }

        webSockets.onclose = function () {
            $scope.Chat.Status = "Desconectado";
            $scope.Chat.MensagemStatus = "Conectar";
        }
    }
})

