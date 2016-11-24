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

        public Usuario ObterPorChave(string chave, bool naoPermitirNulo = false)
        {
            var usuario = _repositorioUsuario.ObterPorChave(chave, naoPermitirNulo);
            return usuario;
        }

        public Usuario ObterPorLogin(string login, bool naoPermitirNulo = false)
        {
            var usuario = _repositorioUsuario.ObterPorLogin(login, naoPermitirNulo);
            return usuario;
        }

        public Usuario ConectarUsuario(string chaveAcesso, string chaveSessao)
        {
            var usuarioSalvo = _repositorioUsuario.ObterPorChave(chaveAcesso);
            if (usuarioSalvo == null)
            {
                throw new SessaoException("Sessão Expirada");
            }
            usuarioSalvo.Disponivel = true;

            if (usuarioSalvo.SessaoWebSocketsAtiva != null)
            {
                if (usuarioSalvo.SessaoWebSocketsAtiva.ChaveClienteWebSokets != chaveSessao)
                {
                    usuarioSalvo.SessaoWebSocketsAtiva.ChaveClienteWebSokets = chaveSessao;
                }
            }
            else
            {
                var sessaoSalva = _servicoSessaoWebSockets.Inserir(chaveSessao, usuarioSalvo);
                usuarioSalvo.SessaoWebSocketsAtiva = sessaoSalva;
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

        public Usuario Atualizar(int id, string nome, string email, string telefone)
        {
            var usuario = _repositorioUsuario.Obter(id);

            var disponivel = !usuario.Atendente ? false : true;

            usuario.Nome = nome;
            usuario.Email = email;
            usuario.Telefone = telefone;
            usuario.Disponivel = disponivel;

            _repositorioUsuario.Atualizar(usuario);

            return usuario;
        }

        public void RemoverSessaoDoUsuario(Usuario usuario)
        {
            //usuario.ChaveAcesso = null;
            usuario.SessaoWebSocketsAtiva = null;
            _repositorioUsuario.Atualizar(usuario);
        }
    }
}
