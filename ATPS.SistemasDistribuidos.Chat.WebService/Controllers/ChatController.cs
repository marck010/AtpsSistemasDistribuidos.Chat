﻿using ATPS.SistemasDistribuidos.Chat.WebService.App_Start.WebSocketsConfiguracao;
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

namespace ATPS.SistemasDistribuidos.Chat.WebService.Controllers
{
    public class ChatController : Controller
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
            return Json(new { ChaveExterna = usuarioAtendente.Usuario.ChaveAcesso }, JsonRequestBehavior.AllowGet);
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
            var atendenteSalvo = _atendenteServico.Inserir(atendente.Nome, atendente.Email, atendente.Telefone, atendente.Login, atendente.Senha);
            var retorno = new
            {
                ChaveAcesso = atendenteSalvo.Usuario.ChaveAcesso
            };
            return Json(retorno, JsonRequestBehavior.AllowGet);
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
