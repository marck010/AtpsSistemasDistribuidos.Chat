using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Mensagem
    {
        protected Mensagem()
        {

        }

        public Mensagem(string texto, Usuario remetente, Usuario destinatario)
        {
            Texto = texto;
            DataEnvio = DateTime.Now;
            Remetente = remetente;
            Destinatario = destinatario;
        }

        public virtual string Texto { get; set; }
        public virtual bool Recebida { get; set; }

        public virtual DateTime DataEnvio { get; set; }
        public virtual DateTime DataRecebimento { get; set; }
        public virtual DateTime DataVisualizacao { get; set; }

        public virtual Atendimento Atendimento { get; set; }
        public virtual Usuario Remetente { get; set; }
        public virtual Usuario Destinatario { get; set; }


        public override  void Validar()
        {
            var erros = new List<string>();

            if (String.IsNullOrWhiteSpace(Texto))
            {
                erros.Add("O Texto deve ser informado.");
            }

            if (DataEnvio == default(DateTime))
            {
                erros.Add("O DataHora deve ser informado.");
            }

            if (Remetente == null)
            {
                erros.Add("O Remetente deve ser informado.");
            }

            if (Destinatario == null)
            {
                erros.Add("O Destinatario deve ser informado.");
            }
            if (erros.Any())
            {
                throw new ValidacaoException(erros);
            }
        }
    }
}