using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioUsuario: RepositorioBaseIntegrador<Usuario>, IRepositorioUsuario
    {
        public IList<Usuario> UsuariosAguardandoAtendimento()
        {
            return Todos().Where(x=>x.Disponivel && !x.Atendente).ToList();
        }

        public IList<Usuario> AtendentesDisponiveis()
        {
            return Todos().Where(x => x.Disponivel && x.Atendente).ToList();
        }

        public Usuario ObterPorChave(string chave)
        {
            var usuario = Todos().FirstOrDefault(u => u.ChaveAcesso== chave);
            return usuario;
        }

        public Usuario ObterPorLogin(string login)
        {
            var usuario = Todos().FirstOrDefault(u => u.Login == login);
            return usuario;
        }

        public Usuario ObterPorNome(string nome)
        {
            var usuario = Todos().FirstOrDefault(u => u.Nome == nome);
            return usuario;
        }
    }
}
