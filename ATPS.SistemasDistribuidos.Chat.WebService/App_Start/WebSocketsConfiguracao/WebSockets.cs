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
        string nomeRemetente;
        string chave;
        private static WebSocketCollection webSocketClient;

        public WebSockets(string remetente)
        {
            nomeRemetente = remetente; 
            if (webSocketClient == null)
            {
                webSocketClient = new WebSocketCollection();
            }
        }

        public override void OnOpen()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            chave = WebSocketContext.SecWebSocketKey;

            ChatEnvio.Instancia.ConectarUsuario(nomeRemetente, chave);

            webSocketClient.Add(this);

            foreach (var cliente in webSocketClient)
            {
                var chaveCliente = cliente.WebSocketContext.SecWebSocketKey;

                var destinatario = ChatEnvio.Instancia.ObterUsuarioPorChave(chaveCliente);
                if (destinatario ==null)
                {
                    continue;
                }
                var mensagem = ChatEnvio.Instancia.Enviar(nomeRemetente, destinatario.Nome, nomeRemetente + " acabou de entrar.");

                var json = JsonConvert.SerializeObject(mensagem, settings);

                if (cliente.WebSocketContext.SecWebSocketKey == this.WebSocketContext.SecWebSocketKey)
                {
                    webSocketClient.Broadcast(json);
                }
                else
                {
                    cliente.Send(json);
                }
            }
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

            var nomeDestinatario = HttpContext.Current.Request.QueryString.Get("destinatario");

            var destinatario = ChatEnvio.Instancia.ObterUsuarioPorNome(nomeDestinatario);
            var clienteDestino = webSocketClient.FirstOrDefault(x => x.WebSocketContext.SecWebSocketKey == destinatario.ChaveConexao);

            var mensagem = ChatEnvio.Instancia.Enviar(nomeRemetente, nomeDestinatario, message);

            var json = JsonConvert.SerializeObject(mensagem, settings);

            if (clienteDestino.WebSocketContext.SecWebSocketKey == this.WebSocketContext.SecWebSocketKey)
            {
                webSocketClient.Broadcast(json);
            }
            else
            {
                clienteDestino.Send(json);
            }
        }

        public override void OnClose()
        {
            base.OnClose();
        }
    }
}