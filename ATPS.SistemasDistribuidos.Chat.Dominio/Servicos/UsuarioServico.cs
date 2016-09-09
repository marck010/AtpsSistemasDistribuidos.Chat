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
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IRepositorioUsuario _repositorioUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioUsuario>();

        public Usuario ObterPorChave(string chave)
        {
            var usuario = _repositorioUsuario.ObterPorChave(chave);
            return usuario;
        }

        public Usuario ObterPorNome(string nome)
        {
            var usuario = _repositorioUsuario.ObterPorNome(nome);
            return usuario;
        }

        public void ConectarUsuario(string remetente, string chave)
        {
            var usuarioSalvo = _repositorioUsuario.ObterPorNome(remetente);

            if (usuarioSalvo == null)
            {
                var novoUsuario = new Usuario { Nome = remetente, ChaveConexao = chave };
                _repositorioUsuario.Inserir(novoUsuario);
            }
            else
            {
                if (usuarioSalvo.ChaveConexao != chave)
                {
                    usuarioSalvo.ChaveConexao = chave;
                    _repositorioUsuario.Atualizar(usuarioSalvo);
                }
            }
        }
    }
}
