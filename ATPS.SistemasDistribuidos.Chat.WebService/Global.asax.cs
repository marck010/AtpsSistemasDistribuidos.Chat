﻿using ATPS.SistemasDistribuidos.Chat.IOC;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using ATPS.SistemasDistribuidos.Dominio.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace ATPS.SistemasDistribuidos.Chat.WebService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var injetorDependencia = InjetorDependencia.Instancia;
            ResolvedorDependenciaDominio.Instancia.Configurar(injetorDependencia);

            var _servicoAtendimento = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoAtendente>();
            
            _servicoAtendimento.AdicionarAdministradorSeNaoExistir();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}