using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.IOC;
using ATPS.SistemasDistribuidos.Dominio.Servicos;

namespace ATPS.SistemasDistribuidos.Chat.IOC
{
    public class InjetorDependencia : IResolvedorDependencia
    {
        private readonly Nucleo _nucleo;

        private static object _lock = new object();
        private static InjetorDependencia _instacia;
        public static InjetorDependencia Instancia
        {
            get
            {
                if (_instacia == null)
                {
                    lock (_lock)
                    {
                        _instacia = new InjetorDependencia();
                    }
                }
                return _instacia;
            }
        }

        public InjetorDependencia()
        {
            _nucleo = new Nucleo();
            Configurar();
        }

        public void Configurar()
        {
            _nucleo.Registrar<IRepositorioAtendente, RepositorioAtendente>();
            _nucleo.Registrar<IRepositorioAtendimento, RepositorioAtendimento>();
            _nucleo.Registrar<IRepositorioSessaoWebSockets, RepositorioSessaoWebSockets>();
            _nucleo.Registrar<IRepositorioUsuario, RepositorioUsuario>();

            _nucleo.Registrar<IRepositorioBase, RepositorioBaseSql>();

            _nucleo.Registrar<IServicoAtendente, ServicoAtendente>();
            _nucleo.Registrar<IServicoAtendimento, ServicoAtendimento>();
            _nucleo.Registrar<IServicoSessaoWebSockets, ServicoSessaoWebSockets>();
            _nucleo.Registrar<IServicoUsuario, ServicoUsuario>();
        }

        public T Resolver<T>()
        {
            return _nucleo.Resolver<T>();
        }
    }
}
