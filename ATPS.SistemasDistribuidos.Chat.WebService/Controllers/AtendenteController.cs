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

namespace ATPS.SistemasDistribuidos.Chat.WebService.Controllers
{
    public class AtendenteController : ApiController
    {
        public AtendenteController()
        {
            _settings = ConfigurarSerializacao();
        }

        private readonly IServicoUsuario _usuarioServico = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoUsuario>();
        private readonly IServicoAtendente _atendenteServico = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoAtendente>();
        private readonly JsonSerializerSettings _settings;

        public HttpResponseMessage Get()
        {
       
            return Request.CreateResponse(HttpStatusCode.OK, "ié ié pegadinha do malandro");

        }

        public HttpResponseMessage EntrarCliente(string chaveAcesso)
        {
            if (HttpContext.Current.IsWebSocketRequest || HttpContext.Current.IsWebSocketRequestUpgrading)
            {
                var instancia = new WebSockets(chaveAcesso);
                HttpContext.Current.AcceptWebSocketRequest(instancia);
            };

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        public HttpResponseMessage EntrarAtendente(string login, string senha)
        {
            if (HttpContext.Current.IsWebSocketRequest || HttpContext.Current.IsWebSocketRequestUpgrading)
            {
                _atendenteServico.Autenticar(login, senha);

                var usuarioAtendente = _usuarioServico.ObterPorLogin(login);
                var instancia = new WebSockets(usuarioAtendente.ChaveAcesso);
                HttpContext.Current.AcceptWebSocketRequest(instancia);
            };

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        //public HttpResponseMessage PostCadastroCliente([FromBody]PessoaModel usuario)
        //{
        //    var usuarioSalvo = _usuarioServico.Inserir(usuario.Nome, usuario.Email, usuario.Telefone);
        //    return Request.CreateResponse(HttpStatusCode.OK, new { Nome = usuarioSalvo.Nome, ChaveAcesso = usuarioSalvo.ChaveAcesso });
        //}
        
        public HttpResponseMessage PostCadastroAtendente(PessoaModel atendente)
        {
            //var atendente = JsonConvert.DeserializeObject<PessoaModel>(atendenteJson, _settings); 
            var atendenteSalvo = _atendenteServico.Inserir(atendente.Nome, atendente.Email, atendente.Telefone, atendente.Login, atendente.Senha);
            return Request.CreateResponse(HttpStatusCode.OK, atendenteSalvo);
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
