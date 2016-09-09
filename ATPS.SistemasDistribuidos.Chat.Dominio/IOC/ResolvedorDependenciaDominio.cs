using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.IOC
{
   public class ResolvedorDependenciaDominio
    {
        private static object _lock = new object();
        private static ResolvedorDependenciaDominio _instancia;

        private IResolvedorDependencia _resolvedorDependencia;

        private ResolvedorDependenciaDominio()
        {
        }

        public static ResolvedorDependenciaDominio Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    lock (_lock)
                    {
                        _instancia = new ResolvedorDependenciaDominio();
                    }
                }
                return _instancia;
            }
        }

        public  void Configurar(IResolvedorDependencia resolvedorDependencia)
        {
            if (_resolvedorDependencia == null)
            {
                lock (_lock)
                {
                    _resolvedorDependencia = resolvedorDependencia;
                }
            }
        }

        public T Resolver<T>()
        {
            return _resolvedorDependencia.Resolver<T>();
        }
    }
}
