using ATPS.SistemasDistribuidos.Chat.WebService;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATPS.SistemasDistribuidos.Chat.Dominio;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Dominio.Servicos;
using ATPS.SistemasDistribuidos.Chat.Dependencias;

namespace ATPS.SistemasDistribuidos.Chat.WebService.App_Start.WebSocketsConfiguracao
{
    public class WebSockets : WebSocketHandler
    {
        string nomeRemetente;
        private readonly IConversaServico _conversaServico = ResolvedorDependencia.Instancia.Resolver<IConversaServico>();
        private readonly IUsuarioServico _usuarioServico = ResolvedorDependencia.Instancia.Resolver<IUsuarioServico>();

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
            var chave = WebSocketContext.SecWebSocketKey;
            _usuarioServico.ConectarUsuario(nomeRemetente, chave);

            var conversas = new List<Conversa>();

            var settings = ConfigurarSerializacao();

            foreach (var cliente in webSocketClient)
            {
                var chaveCliente = cliente.WebSocketContext.SecWebSocketKey;
                var destinatario = _usuarioServico.ObterPorChave(chaveCliente);

                if (destinatario == null)
                {
                    continue;
                }

                var conversa = _conversaServico.Enviar(nomeRemetente, destinatario.Nome, "Acabou de entrar.", new Guid());

                var json = JsonConvert.SerializeObject(new { Conversas = new List<Conversa> { conversa } }, settings);

                cliente.Send(json);

                var conversa2 = _conversaServico.Enviar(destinatario.Nome, nomeRemetente, "Acabou de entrar.", conversa.Identificador);

                conversas.Add(conversa2);
            }

            webSocketClient.Add(this);

            var usuariosConectados = JsonConvert.SerializeObject(new { Conversas = conversas }, settings);

            this.Send(usuariosConectados);
        }

        public override void OnError()
        {
            base.OnError();
        }

        public override void OnMessage(string message)
        {
            var settings = ConfigurarSerializacao();
            var mensagem = JsonConvert.DeserializeObject<Mensagem>(message, settings);
            var nomeDestinatario = mensagem.Destinatario.Nome;
            var destinatario = _usuarioServico.ObterPorNome(nomeDestinatario);
            var clienteDestino = webSocketClient.FirstOrDefault(x => x.WebSocketContext.SecWebSocketKey == destinatario.ChaveConexao);

            var conversa = _conversaServico.Enviar(nomeRemetente, nomeDestinatario, mensagem.Texto, mensagem.Conversa.Identificador);
            var json = JsonConvert.SerializeObject(new { Conversas = new List<Conversa> { conversa } }, settings);

            if (clienteDestino.WebSocketContext.SecWebSocketKey == this.WebSocketContext.SecWebSocketKey)
            {
                this.Send(json);
            }
            else
            {
                clienteDestino.Send(json);
            }
        }

        private static JsonSerializerSettings ConfigurarSerializacao()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return settings;
        }

        public override void OnClose()
        {
            base.OnClose();
        }
    }
}