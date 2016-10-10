using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Servicos
{
    public class ServicoUsuario : IServicoUsuario
    {
        private readonly IRepositorioUsuario _repositorioUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioUsuario>();
        private readonly IRepositorioSessaoWebSockets _repositorioSessaoWebSockets = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioSessaoWebSockets>();
        private readonly IServicoSessaoWebSockets _servicoSessaoWebSockets = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoSessaoWebSockets>();

        public IList<Usuario> UsuariosAguardandoAtendimento()
        {
            return _repositorioUsuario.UsuariosAguardandoAtendimento();
        }

        public IList<Usuario> AtendentesDisponiveis() 
        {
            return _repositorioUsuario.AtendentesDisponiveis();
        }

        public Usuario ObterPorChave(string chave)
        {
            var usuario = _repositorioUsuario.ObterPorChave(chave);
            return usuario;
        }


        public Usuario ObterPorLogin(string login)
        {
            var usuario = _repositorioUsuario.ObterPorLogin(login);
            return usuario;
        }

        public Usuario ObterPorNome(string nome)
        {
            var usuario = _repositorioUsuario.ObterPorNome(nome);
            return usuario;
        }

        public Usuario ConectarUsuario(string chaveAcesso, string chaveSessao)
        {
            var usuarioSalvo = _repositorioUsuario.ObterPorChave(chaveAcesso);
            usuarioSalvo.Disponivel = true;

            if (usuarioSalvo.UltimaSessaoWebSockets != null)
            {
                if (usuarioSalvo.UltimaSessaoWebSockets.ChaveClienteWebSokets != chaveSessao)
                {
                    usuarioSalvo.UltimaSessaoWebSockets.ChaveClienteWebSokets = chaveSessao;
                }
            }
            else
            {
                var sessaoSalva = _servicoSessaoWebSockets.Inserir(chaveSessao, usuarioSalvo);
                usuarioSalvo.UltimaSessaoWebSockets = sessaoSalva;
            }

            _repositorioUsuario.Atualizar(usuarioSalvo);
            return usuarioSalvo;
        }

        public Usuario Inserir(string nome, string email, string telefone)
        {
            var novoUsuario = new Usuario(nome, email, telefone, atendente: false);
            _repositorioUsuario.Inserir(novoUsuario);
            return novoUsuario;
        }
    }
}
