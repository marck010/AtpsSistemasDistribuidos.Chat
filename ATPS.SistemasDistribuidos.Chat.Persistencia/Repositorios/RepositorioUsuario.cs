using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.Excessoes;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioUsuario : RepositorioBaseIntegrador<Usuario>, IRepositorioUsuario
    {
        public IList<Usuario> UsuariosAguardandoAtendimento()
        {
            return Todos().Where(x => x.SessaoSocketAtiva !=null && x.Disponivel && !x.Atendente).ToList();
        }

        public IList<Usuario> AtendentesDisponiveis()
        {
            return Todos().Where(x => x.SessaoSocketAtiva != null && x.Disponivel && x.Atendente).ToList();
        }

        public Usuario ObterPorChave(string chave, bool naoPermitirNulo = false)
        {
            var usuario = Todos().SingleOrDefault(u => u.ChaveAcesso == chave);

            if (usuario == null && naoPermitirNulo)
            {
                throw new ValidacaoException("Usuário não encontrado");
            }

            return usuario;
        }

        public Usuario ObterPorLogin(string login, bool naoPermitirNulo = false)
        {
            var usuario = Todos().SingleOrDefault(u => u.Login == login);

            if (usuario == null && naoPermitirNulo)
            {
                throw new ValidacaoException("Usuário não encontrado");
            }

            return usuario;
        }
    }
}
