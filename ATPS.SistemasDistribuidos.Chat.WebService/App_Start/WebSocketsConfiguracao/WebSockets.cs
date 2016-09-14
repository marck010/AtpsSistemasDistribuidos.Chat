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
        private readonly string _nomeUsuario;
        private readonly IConversaServico _conversaServico = ResolvedorDependencia.Instancia.Resolver<IConversaServico>();
        private readonly IUsuarioServico _usuarioServico = ResolvedorDependencia.Instancia.Resolver<IUsuarioServico>();
        private readonly JsonSerializerSettings _settings;

        private static WebSocketCollection webSocketClient;

        public WebSockets(string nomeUsuario)
        {
            _nomeUsuario = nomeUsuario;
            _settings = ConfigurarSerializacao();

            if (webSocketClient == null)
            {
                webSocketClient = new WebSocketCollection();
            }
        }

        public override void OnOpen()
        {
            var chaveSessaoWebSockets = WebSocketContext.SecWebSocketKey;
            _usuarioServico.ConectarUsuario(_nomeUsuario, chaveSessaoWebSockets);

            var remetente = _usuarioServico.ObterPorNome(_nomeUsuario);

            webSocketClient.Add(this);

            var conversas = new List<Conversa>();

            foreach (var cliente in webSocketClient)
            {
                if (chaveSessaoWebSockets != cliente.WebSocketContext.SecWebSocketKey)
                {
                    var destinatario = _usuarioServico.ObterPorChave(cliente.WebSocketContext.SecWebSocketKey);
                    var respostaParaDestinatario = JsonConvert.SerializeObject(new { Usuario = new { Nome = remetente.Nome, Foto = remetente.Foto } }, _settings);
                    cliente.Send(respostaParaDestinatario);

                    var respostaParaRemetente = JsonConvert.SerializeObject(new { Usuario = new { Nome = destinatario.Nome, Foto = destinatario.Foto } }, _settings);
                    this.Send(respostaParaRemetente);
                }
            }

        }

        public override void OnError()
        {
            base.OnError();
        }

        public override void OnMessage(string mensagem)
        {
            var mensagemDesserializada = JsonConvert.DeserializeObject<Mensagem>(mensagem, _settings);
            var nomeDestinatario = mensagemDesserializada.Destinatario.Nome;
            var usuarioDestinatario = _usuarioServico.ObterPorNome(nomeDestinatario);
            var conexaoClienteDestino = webSocketClient.FirstOrDefault(x => x.WebSocketContext.SecWebSocketKey == usuarioDestinatario.ChaveConexao);

            var conversa = _conversaServico.Enviar(_nomeUsuario, nomeDestinatario, mensagemDesserializada.Texto, mensagemDesserializada.Conversa.Identificador);
            var responstaParaDestinatario = JsonConvert.SerializeObject(new { Usuario = new { Nome = _nomeUsuario }, Conversa = conversa }, _settings);
            conexaoClienteDestino.Send(responstaParaDestinatario);
             
            var responstaParaRemetente= JsonConvert.SerializeObject(new { Usuario = new { Nome = usuarioDestinatario.Nome }, Conversa = conversa }, _settings);
            Send(responstaParaRemetente);
        }

        private JsonSerializerSettings ConfigurarSerializacao()
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