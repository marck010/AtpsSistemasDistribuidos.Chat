using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatEnvio = ATPS.SistemasDistribuidos.Chat.WebService.Models.Chat;

namespace ATPS.SistemasDistribuidos.Chat.WebService.App_Start.WebSocketsConfiguracao
{
    public class WebSockets : WebSocketHandler
    {
        private readonly WebSocketCollection webSocketClient = new WebSocketCollection();
        string nomeRemetente;
        string nomeDestinatario;

        public WebSockets(string remetente, string destinatario)
        {
            nomeRemetente = remetente;
            nomeDestinatario = destinatario;
        }
        public override void OnOpen()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            webSocketClient.Add(this);

            var mensagem = ChatEnvio.Instancia.Enviar(nomeRemetente, nomeDestinatario, nomeRemetente + " acabou de entrar.");
            var json = JsonConvert.SerializeObject(mensagem, settings);

            webSocketClient.Broadcast(json);
        }

        public override void OnError()
        {
            base.OnError();
        }

        public override void OnMessage(string message)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var mensagem = ChatEnvio.Instancia.Enviar(nomeRemetente, nomeDestinatario, message);
            var json = JsonConvert.SerializeObject(mensagem, settings);
            webSocketClient.Broadcast(json);
        }

        public override void OnClose()
        {
            base.OnClose();
        }
    }
}