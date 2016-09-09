using ATPS.SistemasDistribuidos.Chat.Dependencias;
using ATPS.SistemasDistribuidos.Chat.IOC;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ATPS.SistemasDistribuidos.Chat.WebService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            var injetorDependencia = InjetorDependencia.Instancia;

            ResolvedorDependencia.Instancia.Configurar(injetorDependencia);
            ResolvedorDependenciaDominio.Instancia.Configurar(injetorDependencia);

        }
    }
}