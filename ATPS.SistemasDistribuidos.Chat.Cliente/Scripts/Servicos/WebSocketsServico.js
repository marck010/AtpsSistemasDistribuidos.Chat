
moduloChat.factory('$webSocket', function () {
    var webSockets;

    var Conectar = function (chaveAcesso) {
        if (!webSockets || webSockets.readyState != WebSocket.OPEN) {
            CriarWebSockets(chaveAcesso);
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

    var Enviar = function (login, mensagem, idConversa) {

        if (ConexaoAberta()) {
            webSockets.send(" {Conversa:{Identificador:'" + idConversa + "'}, Destinatario:{Login:'" + login + "'}, Texto:'" + mensagem + "'}");
        }

    }

    function ConexaoAberta() {
        return webSockets.readyState == WebSocket.OPEN;
    }

    function CriarWebSockets(chaveAcesso) {
        if (!chaveAcesso) {
            alert("Favor informar sua chave de acesso");
            return;
        }

        webSockets = new WebSocket(urlWs + "Chat/AcessarChat?chaveAcesso=" + chaveAcesso);
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