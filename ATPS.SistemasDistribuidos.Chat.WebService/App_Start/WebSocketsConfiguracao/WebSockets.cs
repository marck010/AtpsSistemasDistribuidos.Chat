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

        public WebSockets(string chaveAcesso)
        {
            _chaveAcesso = chaveAcesso;
            _settings = ConfigurarSerializacao();

            if (webSocketClient == null)
            {
                webSocketClient = new WebSocketCollection();

            }
        }

        #endregion

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
                    EnviarParaAtendenteClientesAguardandoAtendimento();
                    EnviarParaAtendenteAtendimentosInicializados(usuarioConectado);
                }
                else
                {
                    EnviarParaAtendentesNovaConexao(usuarioConectado);
                }

            }
            catch (SessaoException ex)
            {
                webSocketClient.Remove(this);
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

        private void EnviarParaAtendenteAtendimentosInicializados(Usuario usuarioConectado)
        {
            var atendimentosDeUsuarioAtivos = usuarioConectado.Atendimentos.Where(x => x.Cliente.SessaoSocketAtiva != null);

            foreach (var atendimento in atendimentosDeUsuarioAtivos)
            {
                var objetoRespostaParaAtendente = ObjetoResposta(atendimento.Cliente, atendimento);

                var objetoRespostaParaCliente = ObjetoResposta(atendimento.Atendente.Usuario, atendimento);

                var respostaParaAtendente = JsonConvert.SerializeObject(objetoRespostaParaAtendente, _settings);

                var respostaParaCliente = JsonConvert.SerializeObject(objetoRespostaParaCliente, _settings);
                
                var clienteWebSocketDestinatario = ObterClienteWebSocket(atendimento.Cliente);

                clienteWebSocketDestinatario.Send(respostaParaCliente);

                Send(respostaParaAtendente);
            }
        }

        private void EnviarParaAtendenteClientesAguardandoAtendimento()
        {
            var clientesAguardandoAtendimento = _servicoUsuario.UsuariosAguardandoAtendimento();
            foreach (var usuarioAguardandoAtendimento in clientesAguardandoAtendimento)
            {
                var objetoResposta = ObjetoResposta(usuarioAguardandoAtendimento);
                var respostaParaAtendente = JsonConvert.SerializeObject(objetoResposta, _settings);

                Send(respostaParaAtendente);
            }
        }

        private void EnviarParaAtendentesNovaConexao(Usuario usuarioConectado)
        {
            var atendentesDisponiveis = _servicoUsuario.AtendentesDisponiveis();
            var atendimento = usuarioConectado.Atendimentos.OrderBy(x => x.DataHora).LastOrDefault();

            foreach (var atendenteDisponivel in atendentesDisponiveis)
            {
                var atendimentoIniciado = atendenteDisponivel.Atendimentos.LastOrDefault(x => x.Atendente.Usuario.Login == usuarioConectado.Login);
                EnviarMensagem(usuarioConectado, atendenteDisponivel, atendimentoIniciado);
            }

            if (atendimento != null)
            {
                var objetoRespostaCliente = ObjetoResposta(atendimento.Atendente.Usuario, atendimento);
                var respostaParaCliente = JsonConvert.SerializeObject(objetoRespostaCliente, _settings);
                Send(respostaParaCliente);
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

                usuarioDestinatario = _servicoUsuario.ObterPorLogin(mensagem.Destinatario.Login);

                if (usuarioDestinatario == null)
                {
                    throw new ValidacaoException("Destinatário não encontrado.");
                }

                var atendimento = _servicoAtendimento.SalvarAtualizarAtendimento(_chaveAcesso, usuarioDestinatario, mensagem.Texto, mensagem.Atendimento);

                var usuarioRementente = _servicoUsuario.ObterPorChave(_chaveAcesso);

                EnviarMensagem(usuarioRementente, usuarioDestinatario, atendimento);

                var objetoRespostaParaRemetente = ObjetoResposta(usuarioDestinatario, atendimento);
                var responstaParaRemetente = JsonConvert.SerializeObject(objetoRespostaParaRemetente, _settings);

                Send(responstaParaRemetente);
            }
            catch (SessaoException ex)
            {
                webSocketClient.Remove(this);
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
            var usuarioConectado = _servicoUsuario.ObterPorChave(_chaveAcesso);

            if (usuarioConectado != null)
            {
                _servicoUsuario.RemoverSessaoDoUsuario(usuarioConectado);
                webSocketClient.Remove(this);

                if (usuarioConectado.Atendente)
                {
                    var atendimentosDeUsuarioAtivos = usuarioConectado.Atendimentos.Where(x => x.Cliente.SessaoSocketAtiva != null);
            
                    foreach (var item in atendimentosDeUsuarioAtivos)
	                {
                        EnviarMensagem(usuarioConectado, item.Cliente, null);
	                } 
                }
                else
                {
                    var atendimentosDeAtendentesAtivos = _servicoUsuario.AtendentesDisponiveis();

                    foreach (var atendente in atendimentosDeAtendentesAtivos)
                    {
                        EnviarMensagem(usuarioConectado, atendente, null);
                    } 
                }
            }
            
            base.OnClose();
        }


        #endregion

        #region Tratamento de Retorno de Mensagem

        private static object ObjetoResposta(Usuario usuario, Atendimento conversa = null)
        {
            var atendimento = conversa != null ? new
                                                {
                                                    Id = conversa.Id,
                                                    Mensagens = conversa.Mensagens
                                                                        .Select(mensagem =>
                                                                                    new
                                                                                    {
                                                                                        Texto = mensagem.Texto,
                                                                                        Remetente = new
                                                                                        {
                                                                                            Nome = mensagem.Remetente.Nome,
                                                                                            mensagem.Remetente.ChaveAcesso
                                                                                        }
                                                                                    }),
                                                    Login = usuario.Login
                                                } : null;

            object objetoResposta = new
            {
                Usuario = new
                {
                    Nome = usuario.Nome,
                    Login = usuario.Login
                },
                Conversa = atendimento,
                UsuarioDesconectado = usuario.SessaoSocketAtiva == null
            };

            return objetoResposta;
        }

        private void EnviarMensagem(Usuario remetente, Usuario destinatario, Atendimento atendimentoIniciado)
        {
            var objetoRespostaAtendente = ObjetoResposta(remetente, atendimentoIniciado);

            var respostaParaAtendente = JsonConvert.SerializeObject(objetoRespostaAtendente, _settings);
            if (destinatario.SessaoSocketAtiva != null)
            {
                var sessaoDestinatario = ObterClienteWebSocket(destinatario);

                if (sessaoDestinatario != null)
                {
                    sessaoDestinatario.Send(respostaParaAtendente);
                }
                else
                {
                    _servicoUsuario.RemoverSessaoDoUsuario(destinatario);
                }
            }
            if (destinatario.SessaoSocketAtiva == null)
            {
                throw new ValidacaoException("Destinatário não conectado.");
            }
        }

        private static WebSocketHandler ObterClienteWebSocket(Usuario usuario)
        {
            var sessaoDestinatario = webSocketClient.SingleOrDefault(x => x.WebSocketContext.SecWebSocketKey == usuario.SessaoSocketAtiva.Chave);
            return sessaoDestinatario;
        }

        private void RetornarErro(Exception ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.NaoTratado, StackTrace = ex.StackTrace });
            this.Send(error);
        }

        private void RetornarErro(SessaoException ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.SessaoExpirada, StackTrace = ex.StackTrace });
            this.Send(error);
        }

        private void RetornarErro(ValidacaoException ex)
        {
            var error = JsonConvert.SerializeObject(new { Error = ex.Message, TipoErro = TipoErroEnum.ErroTratado, StackTrace = ex.StackTrace });
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