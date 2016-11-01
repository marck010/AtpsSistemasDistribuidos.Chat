using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioAtendente : IRepositorioBaseIntegrador<Atendente>
    {
        Atendente ObterPorChave(string chave, bool naoPermitirNulo = false);
        Atendente ObterPorLogin(string login, bool naoPermitirNulo = false);
    }
}
