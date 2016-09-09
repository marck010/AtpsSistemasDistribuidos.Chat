using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Conversa : EntidadeBase
    {
        public Conversa()
        {
            Mensagens = new List<Mensagem>();
        }

        public override object Id { get { return Identificador; } }

        public Guid Identificador { get; set; }
        public List<Mensagem> Mensagens { get; set; }
    }
}