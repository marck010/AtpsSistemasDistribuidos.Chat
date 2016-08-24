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
using System.Web.Mvc;
using Microsoft.Web.WebSockets;
using System.Net.Http;
using System.Net;

namespace ATPS.SistemasDistribuidos.Chat.WebService.Controllers
{
    public class ChatController : ApiController
    {
        //
        // GET: /Chat/
        
        public HttpResponseMessage Get(string remetente, string destinatario)
        {

            if (HttpContext.Current.IsWebSocketRequest || HttpContext.Current.IsWebSocketRequestUpgrading)
            {
                HttpContext.Current.AcceptWebSocketRequest(new WebSockets(remetente, destinatario));
            };

           return  new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

    }
}
