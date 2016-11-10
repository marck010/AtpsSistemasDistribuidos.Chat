using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Servicos
{
    public class ServicoAtendente : IServicoAtendente
    {
        private readonly IRepositorioAtendente _repositorioAtendente = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioAtendente>();
        private readonly IRepositorioUsuario _repositorioUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioUsuario>();

        public Atendente Inserir(string nome, string email, string telefone, string login, string senha, bool administrador = false) 
        {
            var atendente = new Atendente(nome, email, telefone, login, senha, administrador);
            var atendenteComMesmoLogin = _repositorioAtendente.ObterPorLogin(login);


            if (atendenteComMesmoLogin!=null)
            {
                throw new ValidacaoException("Já existe um atendente com esse login.");
            }
            _repositorioAtendente.Inserir(atendente);
            _repositorioUsuario.Inserir(atendente.Usuario);

            return atendente;
        }
        
        public Atendente Autenticar(string login, string senha)
        {
            var atendente = _repositorioAtendente.ObterPorLogin(login);
            if (atendente == null || atendente.Senha != senha)
            {
                throw new ValidacaoException("Usuario ou senha inválidos");
            }
            return atendente;
        }

        public void AdicionarAdministradorSeNaoExistir()
        {
            string nome = "Administrador", email = "administrador@sac.com.br", telefone = "88889999", login = "administrador", senha = "123456";
            bool administrador = true;

            var atendentes = _repositorioAtendente.Todos();

            if (!atendentes.Any())
            {
                var atendente = new Atendente(nome, email, telefone, login, senha, administrador);

                _repositorioAtendente.Inserir(atendente);
                _repositorioUsuario.Inserir(atendente.Usuario);
            }
  
        }
        
        public Atendente ObterPorChaveAcesso(string chave)
        {
            return _repositorioAtendente.ObterPorChave(chave);
        }
    }
}
