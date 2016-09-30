
moduloChat.controller('ClienteController', function ($scope, $http, $webSocket) {

    $scope.Chat = {};
    $scope.Chat.Atendente;
    $scope.Chat.Remetente = {};
    $scope.Chat.Conectado = false;

    $scope.Chat.Cadastrar = function () {
        var parametros = {
            Nome: $scope.Chat.Remetente.Nome,
            Email: $scope.Chat.Remetente.Email,
            Telefone: $scope.Chat.Remetente.Telefone,
            Login: '',
            Senha: ''
        };

        $http({
            url: urlWsHttp + "Chat/CadastroCliente",
            method: "POST",
            data: JSON.stringify(parametros),
            headers: {
                'Content-Type': 'application/json'
            }
        }).success(function (data) {
            $scope.Chat.Remetente.ChaveAcesso = data.ChaveAcesso;
            $scope.Chat.ConectarDesconectar();
        });
    };

    $scope.Chat.ConectarDesconectar = function () {
        $webSocket.Conectar($scope.Chat.Remetente.ChaveAcesso);

        $webSocket.OnMessage(function (mensagem) {

            var retorno = JSON.parse(mensagem.data);
            if (retorno.Error) {
                alert(retorno.Error)
                return;
            }
            if (!$scope.Chat.Atendente) {
                $scope.Chat.Atendente = retorno;
                $scope.$apply();
            }
            else {
                $scope.Chat.Atendente.Conversa = retorno.Conversa;
                $scope.$apply();
            }
        });

        $webSocket.OnOpen(function () {
            $scope.Chat.Status = "Conectado";
            $scope.Chat.Conectado = true;
            $scope.$apply();
        });

        $webSocket.OnError(function (e) {
            alert(e.data);
        });
    };

    $scope.Chat.Remetente.Enviar = function () {
        if ($scope.Chat.Atendente) {
            alert("Favor informar aguardar o atendimento");
            return;
        }
        if ($scope.Chat.Mensagem) {
            var id = $scope.Chat.Atendente.Conversa.Id;
            $webSocket.Enviar($scope.Chat.Atendente.Login, $scope.Chat.Mensagem, id);
            $scope.Chat.Conversa.Mensagens.push({ Texto: $scope.Chat.Mensagem, Remetente: $scope.Chat.Remetente });
            $scope.Chat.Mensagem = '';
        }
    };
})

