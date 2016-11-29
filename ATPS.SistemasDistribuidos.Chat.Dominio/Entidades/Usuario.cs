using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public class Usuario
    {
        public Usuario()
        {
            Atendimentos = new List<Atendimento>();
            ChaveAcesso = Guid.NewGuid().ToString();
        }

        public Usuario(string nome, string email, string telefone, bool atendente, string login = null): this()
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Atendente = atendente;
            Login = atendente ? login : Guid.NewGuid().ToString();
        }

        public virtual string Nome { get; set; }
        public virtual string Email { get; set; }
        public virtual string Telefone { get; set; }
        public virtual string ChaveAcesso { get; set; }
        public virtual string Login { get; set; }
        public virtual bool Atendente { get; set; }
        public virtual bool Disponivel { get; set; }

        public virtual SessaoClienteSocket SessaoSocketAtiva { get; set; }

        public virtual List<Atendimento> Atendimentos { get; set; }

        public override void Validar()
        {
            var erros = new List<string>();

            if (String.IsNullOrWhiteSpace(Nome))
            {
                erros.Add("O Nome deve ser informado.");
            }

            if (String.IsNullOrWhiteSpace(Login))
            {
                erros.Add("O Login deve ser informado.");
            }

            if (String.IsNullOrWhiteSpace(Email))
            {
                erros.Add("O Email deve ser informado.");
            }

            if (String.IsNullOrWhiteSpace(ChaveAcesso))
            {
                erros.Add("O ChaveAcesso deve ser informado.");
            }

            if (erros.Any())
            {
                throw new ValidacaoException(erros);
            }
        }
    }
}