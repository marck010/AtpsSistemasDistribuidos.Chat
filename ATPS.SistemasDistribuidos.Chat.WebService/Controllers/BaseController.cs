using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using ATPS.SistemasDistribuidos.Dominio.Servicos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ATPS.SistemasDistribuidos.Chat.Controllers
{
    public class BaseController : Controller
    {
        public IServicoAtendente _servicoAtendente = IOC.InjetorDependencia.Instancia.Resolver<IServicoAtendente>();

        protected override void OnException(ExceptionContext exceptionContext)
        {
            if (exceptionContext.Exception is ValidacaoException)
            {
                exceptionContext.Result = RetornarErro((ValidacaoException)exceptionContext.Exception);
            }

            else if (exceptionContext.Exception is SessaoException)
            {
                exceptionContext.Result = RetornarErro((SessaoException)exceptionContext.Exception);
            }

            else if (exceptionContext.Exception is Exception)
            {
                exceptionContext.Result = RetornarErro(exceptionContext.Exception);
            }

            exceptionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            exceptionContext.ExceptionHandled = true;
        
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName == "CadastroAtendente")
            {
                var chaveAcesso = filterContext.RequestContext.HttpContext.Request.Headers["ChaveAcesso"];
                var atendente = _servicoAtendente.ObterPorChaveAcesso(chaveAcesso);
                if (atendente == null || !atendente.Administrador)
                {
                    //throw new ValidacaoException("Usuário não autorizado para está ação.");
                }
            }
            
            base.OnActionExecuting(filterContext);
        }

        private JsonResult RetornarErro(ValidacaoException ex)
        {
            var error = new { Error = ex.Message, TipoErro = TipoErroEnum.ErroTratado, StackTrace = ex.StackTrace };
            return Json(error,JsonRequestBehavior.AllowGet);
        }

        private JsonResult RetornarErro(SessaoException ex)
        {
            var error = new { Error = ex.Message, TipoErro = TipoErroEnum.SessaoExpirada, StackTrace = ex.StackTrace };
            return Json(error,JsonRequestBehavior.AllowGet);
        }

        private JsonResult RetornarErro(Exception ex)
        {
            var error = new { Error = ex.Message, TipoErro = TipoErroEnum.NaoTratado, StackTrace = ex.StackTrace };
            return Json(error,JsonRequestBehavior.AllowGet);
        }

    }
}