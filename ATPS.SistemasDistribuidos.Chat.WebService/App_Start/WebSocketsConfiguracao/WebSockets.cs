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
using ATPS.SistemasDistribuidos.Dominio.IOC;

namespace ATPS.SistemasDistribuidos.Chat.WebService.App_Start.WebSocketsConfiguracao
{
    public class WebSockets : WebSocketHandler
    {
        private readonly string _chaveAcesso;
        private readonly IServicoAtendimento _servicoAtendimento = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoAtendimento>();
        private readonly IServicoUsuario _servicoUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoUsuario>();
        private readonly IServicoSessaoWebSockets _sessaoWebSocketsServico = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoSessaoWebSockets>();

        private readonly JsonSerializerSettings _settings;

        private WebSocketCollection webSocketClient;

        public WebSockets(string chaveAcesso)
        {
            _chaveAcesso = chaveAcesso;
            _settings = ConfigurarSerializacao();

            if (webSocketClient == null)
            {
                webSocketClient = new WebSocketCollection();
            }
        }

        public override void OnOpen()
        {
            var chaveSessaoWebSocketsRemetente = WebSocketContext.SecWebSocketKey;

            var usuarioConectado = _servicoUsuario.ConectarUsuario(_chaveAcesso, chaveSessaoWebSocketsRemetente);

            var conversas = new List<Atendimento>();
            if (usuarioConectado.Atendente)
            {
                var clientesAguardandoAtendimento = _servicoUsuario.UsuariosAguardandoAtendimento();
                foreach (var usuarioAguardandoAtendimento in clientesAguardandoAtendimento)
                {
                    var objetoResposta = ObjetoResposta(usuarioAguardandoAtendimento);
                    var respostaParaRemetente = JsonConvert.SerializeObject(objetoResposta, _settings);

                    this.Send(respostaParaRemetente);
                }
            }
        }

        public override void OnError()
        {
            var error = JsonConvert.SerializeObject(new { Error = Error.Message });
            this.Send(error);
        }

        public override void OnMessage(string dados)
        {
            var mensagem = JsonConvert.DeserializeObject<Mensagem>(dados, _settings);
            var usuarioDestinatario = _servicoUsuario.ObterPorLogin(mensagem.Destinatario.Login);

            var conversa = _servicoAtendimento.Enviar(_chaveAcesso, usuarioDestinatario.Login, mensagem.Texto, mensagem.Atendimento.Id);

            var usuarioRementente = _servicoUsuario.ObterPorChave(_chaveAcesso);
            object objetoRespostaParaDestinatario = ObjetoResposta(usuarioRementente, conversa);
            var responstaParaDestinatario = JsonConvert.SerializeObject(objetoRespostaParaDestinatario, _settings);

            var conexaoClienteDestino = webSocketClient.SingleOrDefault(x => x.WebSocketContext.SecWebSocketKey == usuarioDestinatario.UltimaSessaoWebSockets.ChaveClienteWebSokets);
            conexaoClienteDestino.Send(responstaParaDestinatario);

            object objetoRespostaParaRemetente = ObjetoResposta(usuarioDestinatario, conversa);
            var responstaParaRemetente = JsonConvert.SerializeObject(objetoRespostaParaRemetente, _settings);

            Send(responstaParaRemetente);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        private static object ObjetoResposta(Usuario usuario, Atendimento conversa = null)
        {
            object objetoResposta = new
            {
                Nome = usuario.Nome,
                Login = usuario.Login,
                Conversa = conversa
            };
            return objetoResposta;
        }

        private JsonSerializerSettings ConfigurarSerializacao()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return settings;
        }
    }
}