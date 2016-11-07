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
        IList<Usuario> UsuariosAguardandoAtendimento();
        IList<Usuario> AtendentesDisponiveis();
        Usuario ObterPorChave(string chave, bool naoPermitirNulo = false);
        Usuario ObterPorLogin(string login, bool naoPermitirNulo = false);
    }
}
