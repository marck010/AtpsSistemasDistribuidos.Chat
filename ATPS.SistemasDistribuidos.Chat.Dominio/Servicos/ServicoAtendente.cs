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

        public Atendente Inserir(string nome, string email, string telefone, string login, string senha) 
        {
            var atendente  = new Atendente(nome, email, telefone, login, senha);
            
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
        
        public Atendente ObterPorChaveAcesso(string chave)
        {
            return _repositorioAtendente.ObterPorChave(chave);
        }
    }
}
