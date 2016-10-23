using ATPS.SistemasDistribuidos.Dominio.Excessoes;
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

        public Atendimento Atendimento { get; set; }
        public Usuario Remetente { get; set; }
        public Usuario Destinatario { get; set; }

        public Mensagem(string texto, Usuario remetente, Usuario destinatario)
        {
            Texto = texto;
            DataEnvio = DateTime.Now;
            Remetente = remetente;
            Destinatario = destinatario;
        }

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