using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.Servicos;
using ATPS.SistemasDistribuidos.Dominio.IOC;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioSessaoWebSockets : RepositorioBaseIntegrador<SessaoClienteWebSockets>, IRepositorioSessaoWebSockets
    {
        public SessaoClienteWebSockets ObterPorChave(string chaveAcesso) 
        {
            return Todos().SingleOrDefault(x => x.Usuario.ChaveAcesso == chaveAcesso);
        }
    }
}
