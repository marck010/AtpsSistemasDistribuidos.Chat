using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Dependencias
{
    public class ResolvedorDependencia
    {
        private static object _lock = new object();
        private static ResolvedorDependencia _instancia;

        private IResolvedorDependencia _resolvedorDependencia;

        private ResolvedorDependencia()
        {
        }

        public static ResolvedorDependencia Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    lock (_lock)
                    {
                        _instancia = new ResolvedorDependencia();
                    }
                }
                return _instancia;
            }
        }

        public void Configurar(IResolvedorDependencia resolvedorDependencia)
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
