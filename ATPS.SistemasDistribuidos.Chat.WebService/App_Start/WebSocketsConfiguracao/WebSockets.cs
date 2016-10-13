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
using ATPS.SistemasDistribuidos.Dominio.Excessoes;

namespace ATPS.SistemasDistribuidos.Chat.WebService.App_Start.WebSocketsConfiguracao
{
    public class WebSockets : WebSocketHandler
    {
        private readonly string _chaveAcesso;
        private readonly IServicoAtendimento _servicoAtendimento = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoAtendimento>();
        private readonly IServicoUsuario _servicoUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoUsuario>();
        private readonly IServicoSessaoWebSockets _sessaoWebSocketsServico = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoSessaoWebSockets>();

        private readonly JsonSerializerSettings _settings;

        private static WebSocketCollection webSocketClient;

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
            try
            {


                var usuarioConectado = _servicoUsuario.ConectarUsuario(_chaveAcesso, chaveSessaoWebSocketsRemetente);

                webSocketClient.Add(this);

                var conversas = new List<Atendimento>();
                if (usuarioConectado.Atendente)
                {
                    var clientesAguardandoAtendimento = _servicoUsuario.UsuariosAguardandoAtendimento();
                    foreach (var usuarioAguardandoAtendimento in clientesAguardandoAtendimento)
                    {
                        var objetoResposta = ObjetoResposta(usuarioAguardandoAtendimento);
                        var respostaParaAtendente = JsonConvert.SerializeObject(objetoResposta, _settings);

                        this.Send(respostaParaAtendente);
                    }

                    foreach (var atendimento in usuarioConectado.Atendimentos)
                    {
                        var objetoResposta = ObjetoResposta(atendimento.ClienteUsuario, atendimento);

                        var respostaParaAtendente = JsonConvert.SerializeObject(objetoResposta, _settings);

                        this.Send(respostaParaAtendente);
                    }
                }
                else
                {
                    var atendentesDisponiveis = _servicoUsuario.AtendentesDisponiveis();

                    foreach (var atendenteDisponivel in atendentesDisponiveis)
                    {
                        var atendimentoIniciado = atendenteDisponivel.Atendimentos.LastOrDefault(x => x.ClienteUsuario.Login == usuarioConectado.Login);

                        var objetoRespostaAtendente = ObjetoResposta(usuarioConectado, atendimentoIniciado);

                        var respostaParaAtendente = JsonConvert.SerializeObject(objetoRespostaAtendente, _settings);

                        var sessaoAtendente = webSocketClient.SingleOrDefault(x => x.WebSocketContext.SecWebSocketKey == atendenteDisponivel.UltimaSessaoWebSockets.ChaveClienteWebSokets);

                        sessaoAtendente.Send(respostaParaAtendente);
                    }

                    var atendimento = usuarioConectado.Atendimentos.OrderBy(x => x.DataHora).LastOrDefault();
                    if (atendimento != null)
                    {
                        var objetoRespostaCliente = ObjetoResposta(atendimento.Atendente.Usuario, atendimento);
                        var respostaParaCliente = JsonConvert.SerializeObject(objetoRespostaCliente, _settings);
                        Send(respostaParaCliente);
                    }
                }

            }
            catch (SessaoException ex)
            {
                RetornarErro(ex);
            }
            catch (Exception ex)
            {
                RetornarErro(ex);
            }
        }

        public void RetornarErro(Exception ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.NaoTratado });
            this.Send(error);
        }

        public void RetornarErro(SessaoException ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.SessaoExpirada });
            this.Send(error);
        }

        public void RetornarErro(ValidacaoException ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.ErroTratado});
            this.Send(error);
        }

        public override void OnError()
        {
            RetornarErro(Error);
        }

        public override void OnMessage(string dados)
        {
            var mensagem = JsonConvert.DeserializeObject<Mensagem>(dados, _settings);
            var usuarioDestinatario = _servicoUsuario.ObterPorLogin(mensagem.Destinatario.Login);

            var conversa = _servicoAtendimento.Enviar(_chaveAcesso, usuarioDestinatario.Login, mensagem.Texto, mensagem.Atendimento);

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
                Usuario = new
                {
                    Nome = usuario.Nome,
                    Login = usuario.Login
                },
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