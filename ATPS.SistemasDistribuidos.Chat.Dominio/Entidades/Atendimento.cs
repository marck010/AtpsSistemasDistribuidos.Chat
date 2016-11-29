using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Atendimento
    {
        public Atendimento()
        {
            Mensagens = new List<Mensagem>();
        }

        public virtual DateTime DataHora { get; set; }

        public virtual Usuario Cliente { get; set; }
        public virtual Atendente Atendente { get; set; }
        public virtual List<Mensagem> Mensagens { get; set; }

        public Atendimento(Usuario cliente, Atendente atendente)
            : this()
        {
            Cliente = cliente;
            Atendente = atendente;
            DataHora = DateTime.Now;
        }

        public override void Validar()
        {
            var erros = new List<string>();

            if (Cliente == null)
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
                throw new ValidacaoException(erros);
            }
        }
    }
}

