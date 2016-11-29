using ATPS.SistemasDistribuidos.Chat.WebService.App_Start.WebSocketsConfiguracao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Web.WebSockets;
using System.Net.Http;
using System.Net;
using ATPS.SistemasDistribuidos.Dominio.Servicos;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using System.Web.Http.Cors;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.WebService.Models;
using Newtonsoft.Json;
using System.Web.Mvc;
using ATPS.SistemasDistribuidos.Chat.Controllers;

namespace ATPS.SistemasDistribuidos.Chat.WebService.Controllers
{
    public class ChatController : BaseController
    {
        public ChatController()
        {
            _settings = ConfigurarSerializacao();
        }

        private readonly IServicoUsuario _usuarioServico = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoUsuario>();
        private readonly IServicoAtendente _atendenteServico = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoAtendente>();
        private readonly JsonSerializerSettings _settings;

        public void AcessarChat(string chaveAcesso)
        {
            if (HttpContext.IsWebSocketRequest || HttpContext.IsWebSocketRequestUpgrading)
            {
                var instancia = new WebSockets(chaveAcesso);
                HttpContext.AcceptWebSocketRequest(instancia);
            };

            HttpContext.Response.StatusCode = (int)HttpStatusCode.SwitchingProtocols;
        }

        public JsonResult AutenticarAtendente(string login, string senha)
        {
            var usuarioAtendente = _atendenteServico.Autenticar(login, senha);
            return Json(new { ChaveAcesso = usuarioAtendente.Usuario.ChaveAcesso, Administrador = usuarioAtendente.Administrador }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CadastroCliente(PessoaModel cliente)
        {
            var usuarioSalvo = _usuarioServico.Inserir(cliente.Nome, cliente.Email, cliente.Telefone);
            var retorno = new
            {
                ChaveAcesso = usuarioSalvo.ChaveAcesso
            };
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CadastroAtendente(PessoaModel atendente)
        {
            var atendenteSalvo = _atendenteServico.Inserir(atendente.Nome, atendente.Email, atendente.Telefone, atendente.Login, atendente.Senha, atendente.Administrador);
            var retorno = new
            {
                ChaveAcesso = atendenteSalvo.Usuario.ChaveAcesso
            };
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterAtendimentos(string chaveAcesso)
        {
            var usuarioConectado = _usuarioServico.ObterPorChave(chaveAcesso);
            object retorno;
            if (usuarioConectado.Atendente)
            {
                var clientesAguardandoAtendimento = _usuarioServico.UsuariosAguardandoAtendimento();
                retorno = clientesAguardandoAtendimento.Select(usuario => ObjetoResposta(usuario, usuario.Atendimentos.LastOrDefault(atendimento => atendimento.Atendente.Usuario.ChaveAcesso == chaveAcesso)));
            }
            else
            {
                var usuarioLogado = _usuarioServico.ObterPorChave(chaveAcesso);
                var objetoResposta = ObjetoResposta(usuarioConectado);
                var respostaParaRemetente = JsonConvert.SerializeObject(objetoResposta, _settings);
                retorno = ObjetoResposta(usuarioLogado, usuarioLogado.Atendimentos.LastOrDefault(atendimento => atendimento.Cliente.ChaveAcesso == chaveAcesso));

            }
            return Json(retorno, JsonRequestBehavior.AllowGet);
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
