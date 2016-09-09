using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.IOC
{
    public interface IResolvedorDependencia
    {
        T Resolver<T>();
    }
}
