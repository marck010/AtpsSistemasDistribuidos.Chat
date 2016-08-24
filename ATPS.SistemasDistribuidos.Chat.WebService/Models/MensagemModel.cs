using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.WebService.Models
{
    public class MensagemModel
    {
        public string Texto { get; set; }
        public DateTime DataEnvio { get; set; }
        public ConversaModel Conversa { get; set; }
    }
}