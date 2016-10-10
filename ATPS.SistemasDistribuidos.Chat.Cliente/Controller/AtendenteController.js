
moduloChat.controller('AtendenteController', function ($scope, $http, $webSocket) {
    function Init() {
        var chaveAcesso = sessionStorage.getItem("ChaveAcesso");
        if (chaveAcesso) {
            $scope.Chat.Remetente.ChaveAcesso = chaveAcesso;
            $scope.Chat.ConectarDesconectar();
        }
        else {
            $scope.Chat.Conectado = false;
        }
    }


    $scope.Chat = {};
    $scope.Chat.Atendimentos = [];
    $scope.Chat.Remetente = {};
    $scope.Chat.Cliente = {};
    $scope.Chat.Conversa = {};
    $scope.Chat.Conversa.Mensagens = [];
    $scope.Chat.Status = "Desconectado";

    $scope.Chat.Cadastrar = function () {
        if ($scope.Chat.Remetente.Senha != $scope.Chat.Remetente.ConfirmarSenha) {
            alert("Senhas não correspondem");
            return;
        }
        var parametros = {
            Nome: $scope.Chat.Remetente.Nome,
            Email: $scope.Chat.Remetente.Email,
            Telefone: $scope.Chat.Remetente.Telefone,
            Login: $scope.Chat.Remetente.Login,
            Senha: $scope.Chat.Remetente.Senha
        };

        $http({
            url: urlWsHttp + "Chat/CadastroAtendente",
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
        sessionStorage.setItem("ChaveAcesso", $scope.Chat.Remetente.ChaveAcesso);
        $webSocket.Conectar($scope.Chat.Remetente.ChaveAcesso);

        $webSocket.OnMessage(function (mensagem) {

            var retorno = JSON.parse(mensagem.data);
            if (retorno.Error) {
                alert(retorno.Error)
                return;
            }

            listarAtendimentos(retorno);
            $scope.$apply();

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

    $scope.Chat.SelecionarDestinatario = function (atendimentoSelecionado) {

        Enumerable.From($scope.Chat.Atendimentos).Where(function (atendimento) {
            return !!atendimento.Selecionado;
        }).ForEach(function (atendimento) {
            atendimento.Selecionado = false;
        });

        atendimentoSelecionado.Selecionado = true;
        if (atendimentoSelecionado.Conversa) {
            $scope.Chat.Conversa = atendimentoSelecionado.Conversa;
        }
        $scope.Chat.Cliente = atendimentoSelecionado.Usuario;

    };

    $scope.Chat.Remetente.Enviar = function () {

        if ($scope.Chat.Mensagem) {

            var idConversa = !$scope.Chat.Conversa || !$scope.Chat.Conversa.Id ? '' : $scope.Chat.Conversa.Id;

            $webSocket.Enviar($scope.Chat.Cliente.Login, $scope.Chat.Mensagem, idConversa);

            $scope.Chat.Conversa.Mensagens.push({ Texto: $scope.Chat.Mensagem, Remetente: $scope.Chat.Remetente });

            $scope.Chat.Mensagem = '';
        }
    };

    var chaveAcesso = sessionStorage.getItem("ChaveAcesso");
    if (chaveAcesso) {
        $scope.Chat.Remetente.ChaveAcesso = chaveAcesso;
        $scope.Chat.ConectarDesconectar();
    }
    else {
        $scope.Chat.Conectado = false;
    }

    Init();

    function listarAtendimentos(retorno) {

        if ($scope.Chat.Atendimentos.length == 0) {
            $scope.Chat.Atendimentos.push(retorno);
        }
        else {
            var atendimentoEmAndamento = Enumerable.From($scope.Chat.Atendimentos).FirstOrDefault(null, function (atendimento) {
                return retorno.Usuario.Login == atendimento.Usuario.Login
            });

            if (atendimentoEmAndamento) {
                if (retorno.Conversa) {
                    atendimentoEmAndamento.Conversa = retorno.Conversa;
                    if (atendimentoEmAndamento.Selecionado) {
                        $scope.Chat.Conversa = atendimentoEmAndamento.Conversa;
                        $scope.Chat.MensagensDestinatario = atendimentoEmAndamento.Conversa.Mensagens;
                    }
                }
            } else {
                $scope.Chat.Atendimentos.push(retorno);
            }
        }
    }
})

