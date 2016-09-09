using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Usuario : EntidadeBase
    {
        public Usuario()
        {
            Conversas = new List<Conversa>();
        }
        
        public string Foto { get; set; }
        public string Nome { get; set; }
        public string ChaveConexao { get; set; }
        public List<Conversa> Conversas { get; set; }

    }
}