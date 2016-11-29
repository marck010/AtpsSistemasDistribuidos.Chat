using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class SessaoClienteSocket
    {
        protected SessaoClienteSocket()
        {

        }

        public SessaoClienteSocket(string chaveClienteWebSockets, Usuario usuario)
        {
            Chave = chaveClienteWebSockets;
            Usuario = usuario;
        }

        public virtual string Chave { get; set; }
        public virtual Usuario Usuario { get; set; }

        public override void Validar()
        {
            var erros = new List<string>();

            if (String.IsNullOrWhiteSpace(Chave))
            {
                erros.Add("O ChaveClienteWebSokets deve ser informado.");
            }

            if (Usuario == null)
            {
                erros.Add("O Usuario deve ser informado.");
            }

            if (erros.Any())
            {
                throw new ValidacaoException(erros);
            }
        }
    }
}