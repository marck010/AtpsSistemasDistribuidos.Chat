using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.WebService.Models
{
    public class UsuarioModel
    {
        public UsuarioModel()
        {
            Conversas = new List<ConversaModel>();
        }
        
        public string Nome { get; set; }
        public string ChaveConexao { get; set; }
        public List<ConversaModel> Conversas { get; set; }

    }
}