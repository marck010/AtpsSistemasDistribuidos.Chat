using ATPS.SistemasDistribuidos.Dominio.Excessoes;
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
            Atendimentos = new List<Atendimento>();
            ChaveAcesso = Guid.NewGuid().ToString();
        }

        public Usuario(string nome,  string email, string telefone, bool atendente, string login = null): this()
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Atendente = atendente;
            Login = atendente ? login : Guid.NewGuid().ToString();
        }

        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string ChaveAcesso { get; set; }
        public string Login { get; set; }
        public bool Atendente { get; set; }
        public bool Disponivel { get; set; }

        public SessaoClienteWebSockets UltimaSessaoWebSockets { get; set; }

        public List<Atendimento> Atendimentos { get; set; }

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
                throw new ValidacaoException(String.Join(Environment.NewLine, erros));
            }
        }
    }
}