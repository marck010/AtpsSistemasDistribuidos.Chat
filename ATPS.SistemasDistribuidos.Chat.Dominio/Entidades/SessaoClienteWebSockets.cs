using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class SessaoClienteWebSockets : EntidadeBase
    {
        public SessaoClienteWebSockets(string chaveClienteWebSockets, Usuario usuario)
        {
            ChaveClienteWebSokets = chaveClienteWebSockets;
            Usuario = usuario;
        }

        public string ChaveClienteWebSokets { get; set; }
        public Usuario Usuario { get; set; }

        public override void Validar()
        {
            var erros = new List<string>();

            if (String.IsNullOrWhiteSpace(ChaveClienteWebSokets))
            {
                erros.Add("O ChaveClienteWebSokets deve ser informado.");
            }

            if (Usuario == null)
            {
                erros.Add("O Usuario deve ser informado.");
            }

            if (erros.Any())
            {
                throw new Exception(String.Join(Environment.NewLine, erros));
            }
        }
    }
}