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
        #region Variáveis de instancia

        private readonly string _chaveAcesso;
        private readonly IServicoAtendimento _servicoAtendimento = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoAtendimento>();
        private readonly IServicoUsuario _servicoUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoUsuario>();
        private readonly IServicoSessaoWebSockets _sessaoWebSocketsServico = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoSessaoWebSockets>();
        private readonly JsonSerializerSettings _settings;
        private static WebSocketCollection webSocketClient;

        #endregion

        #region Construtores

        #endregion
        public WebSockets(string chaveAcesso)
        {
            _chaveAcesso = chaveAcesso;
            _settings = ConfigurarSerializacao();

            if (webSocketClient == null)
            {
                webSocketClient = new WebSocketCollection();
            }
        }

        #region Eventos

        public override void OnOpen()
        {
            try
            {
                var chaveSessaoWebSocketsRemetente = WebSocketContext.SecWebSocketKey;
                var usuarioConectado = _servicoUsuario.ConectarUsuario(_chaveAcesso, chaveSessaoWebSocketsRemetente);
                var conversas = new List<Atendimento>();

                webSocketClient.Add(this);

                if (usuarioConectado.Atendente)
                {
                    var clientesAguardandoAtendimento = _servicoUsuario.UsuariosAguardandoAtendimento();
                    foreach (var usuarioAguardandoAtendimento in clientesAguardandoAtendimento)
                    {
                        var objetoResposta = ObjetoResposta(usuarioAguardandoAtendimento);
                        var respostaParaAtendente = JsonConvert.SerializeObject(objetoResposta, _settings);

                        Send(respostaParaAtendente);
                    }

                    foreach (var atendimento in usuarioConectado.Atendimentos)
                    {
                        var objetoResposta = ObjetoResposta(atendimento.ClienteUsuario, atendimento);

                        var respostaParaAtendente = JsonConvert.SerializeObject(objetoResposta, _settings);

                        Send(respostaParaAtendente);
                    }
                }
                else
                {
                    var atendentesDisponiveis = _servicoUsuario.AtendentesDisponiveis();

                    foreach (var atendenteDisponivel in atendentesDisponiveis)
                    {
                        EnviarMensagemClienteWebSockets(usuarioConectado, atendenteDisponivel);
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
                WebSocketContext.WebSocket.Abort();
                RetornarErro(ex);
            }
            catch (ValidacaoException ex)
            {
                WebSocketContext.WebSocket.Abort();
                RetornarErro(ex);
            }
            catch (Exception ex)
            {
                WebSocketContext.WebSocket.Abort();
                RetornarErro(ex);
            }
        }

        public override void OnError()
        {
            RetornarErro(Error);
        }

        public override void OnMessage(string dados)
        {
            try
            {
                var mensagem = JsonConvert.DeserializeObject<Mensagem>(dados, _settings);
                if (mensagem.Destinatario == null)
                {
                    throw new ValidacaoException("Entrada de dados invalida. É necessário informar o Login do destinatário.");
                }

                Usuario usuarioDestinatario;

                try
                {
                    usuarioDestinatario = _servicoUsuario.ObterPorLogin(mensagem.Destinatario.Login, naoPermitirNulo: true);
                }
                catch (ValidacaoException)
                {
                    throw new ValidacaoException("Destinatário não encontrado");
                }

                var atendimento = _servicoAtendimento.SalvarAtualizarAtendimento(_chaveAcesso, usuarioDestinatario, mensagem.Texto, mensagem.Atendimento);

                var usuarioRementente = _servicoUsuario.ObterPorChave(_chaveAcesso);

                EnviarMensagemClienteWebSockets(usuarioRementente, usuarioDestinatario);

                var objetoRespostaParaRemetente = ObjetoResposta(usuarioDestinatario, atendimento);
                var responstaParaRemetente = JsonConvert.SerializeObject(objetoRespostaParaRemetente, _settings);

                Send(responstaParaRemetente);
            }
            catch (SessaoException ex)
            {
                RetornarErro(ex);
            }
            catch (ValidacaoException ex)
            {
                RetornarErro(ex);
            }
            catch (Exception ex)
            {
                RetornarErro(ex);
            }

        }

        public override void OnClose()
        {
            base.OnClose();
        }


        #endregion

        #region Tratamento de Retorno de Mensagem

        private static object ObjetoResposta(Usuario usuario, Atendimento conversa = null)
        {
            object objetoResposta = new
            {
                Usuario = new
                {
                    Nome = usuario.Nome,
                    Login = usuario.Login
                },
                Conversa = conversa,
            };
            return objetoResposta;
        }

        private bool EnviarMensagemClienteWebSockets(Usuario remetente, Usuario destinatario)
        {
            var atendimentoIniciado = destinatario.Atendimentos.LastOrDefault(x => x.ClienteUsuario.Login == remetente.Login);

            var objetoRespostaAtendente = ObjetoResposta(remetente, atendimentoIniciado);

            var respostaParaAtendente = JsonConvert.SerializeObject(objetoRespostaAtendente, _settings);

            var sessaoAtendente = webSocketClient.SingleOrDefault(x => x.WebSocketContext.SecWebSocketKey == destinatario.UltimaSessaoWebSockets.ChaveClienteWebSokets);
            if (sessaoAtendente != null)
            {
                sessaoAtendente.Send(respostaParaAtendente);
                return true;
            }
            else
            {
                _servicoUsuario.RemoverSessaoDoUsuario(destinatario);
                return false;
            }
        }

        private void RetornarErro(Exception ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.NaoTratado });
            this.Send(error);
        }

        private void RetornarErro(SessaoException ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.SessaoExpirada });
            this.Send(error);
        }

        private void RetornarErro(ValidacaoException ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.ErroTratado });
            this.Send(error);
        }

        #endregion

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