using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Mensagem : EntidadeBase
    {
        public string Texto { get; set; }
        public bool Recebida { get; set; }

        public DateTime DataEnvio { get; set; }
        public DateTime DataRecebimento { get; set; }
        public DateTime DataVisualizacao { get; set; }

        public Conversa Conversa { get; set; }
        public Usuario Remetente { get; set; }
        public Usuario Destinatario { get; set; }
    }
}