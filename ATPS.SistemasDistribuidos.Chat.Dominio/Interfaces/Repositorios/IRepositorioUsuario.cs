using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioUsuario : IRepositorioBaseIntegrador<Usuario>
    {
        Usuario ObterPorChave(string chave);
        Usuario ObterPorNome(string nome);
    }
}
