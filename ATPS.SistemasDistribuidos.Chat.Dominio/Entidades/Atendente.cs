using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Atendente : EntidadeBase
    {
        public Atendente(string nome, string email, string telefone, string login, string senha, bool administrador = false)
        {
            Usuario = new Usuario(nome, email, telefone, atendente: true, login: login);
            Senha = senha;
            Administrador = administrador;
        }

        public string Foto { get; set; }
        public string Senha { get; set; }
        public Usuario Usuario { get; set; }
        public bool Administrador { get; set; }

        public override void Validar()
        {
            var erros = new List<string>();

            if (String.IsNullOrWhiteSpace(Senha))
            {
                erros.Add("A Senha deve ser informada.");
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