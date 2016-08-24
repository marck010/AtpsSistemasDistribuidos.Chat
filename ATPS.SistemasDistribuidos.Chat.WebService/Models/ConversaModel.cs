using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.WebService.Models
{
    public class ConversaModel
    {
        public ConversaModel()
        {
            Mensagens = new List<MensagemModel>();
        }
        public UsuarioModel Remetente { get; set; }
        public UsuarioModel Destinatario { get; set; }
        public List<MensagemModel> Mensagens { get; set; }
    }
}