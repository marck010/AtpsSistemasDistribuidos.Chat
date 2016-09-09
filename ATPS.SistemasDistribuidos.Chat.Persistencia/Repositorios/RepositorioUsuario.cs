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
        public Usuario ObterPorChave(string chave)
        {
            var usuario = Todos().FirstOrDefault(u => u.ChaveConexao == chave);
            return usuario;
        }

        public Usuario ObterPorNome(string nome)
        {
            var usuario = Todos().FirstOrDefault(u => u.Nome == nome);
            return usuario;
        }
    }
}
