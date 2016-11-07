using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ATPS.SistemasDistribuidos.Chat.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext exceptionContext)
        {
            if (exceptionContext.Exception is ValidacaoException)
            {
                exceptionContext.Result = RetornarErro((ValidacaoException)exceptionContext.Exception);
            }

            if (exceptionContext.Exception is SessaoException)
            {
                exceptionContext.Result = RetornarErro((SessaoException)exceptionContext.Exception);
            }

            if (exceptionContext.Exception is Exception)
            {
                exceptionContext.Result = RetornarErro(exceptionContext.Exception);
            }
            
            base.OnException(exceptionContext);
        
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