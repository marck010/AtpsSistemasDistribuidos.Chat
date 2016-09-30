using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Atendimento :EntidadeBase
    {
        public Atendimento()
        {
            Mensagens = new List<Mensagem>();
        }

        public DateTime DataHora{ get; set; }

        public Usuario ClienteUsuario { get; set; }
        public Atendente Atendente { get; set; }
        public List<Mensagem> Mensagens { get; set; }

        public Atendimento(Usuario cliente, Atendente atendente)
        {
            ClienteUsuario = cliente;
            Atendente = atendente;
            DataHora = DateTime.Now;
        }

        public override void Validar() {
            var erros = new List<string>();

            if (ClienteUsuario == null)
            {
                erros.Add("O cliente deve ser informado.");
            }

            if (Atendente == null)
            {
                erros.Add("O Atendente deve ser informado.");
            }

            if (DataHora == default(DateTime))
            {
                erros.Add("O DataHora deve ser informado.");
            }

            if (erros.Any())
            {
                throw new Exception(String.Join(Environment.NewLine, erros));
            }
        }
    }
}

