using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Servicos
{
    public class ServicoSessaoWebSockets : IServicoSessaoWebSockets
    {
        private readonly IRepositorioSessaoWebSockets _repositorioSessaoWebSockets = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioSessaoWebSockets>();
        private readonly IRepositorioUsuario _repositorioUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioUsuario>();
        private static object _lock;
        
        static ServicoSessaoWebSockets()
        {
            _lock = new object();
        }

        public SessaoClienteWebSockets Inserir(string chaveSessaoWebSokets, Usuario usuario)
        {
            lock (_lock)
            {
                var clienteWebSockets = new SessaoClienteWebSockets(chaveSessaoWebSokets, usuario);

                var sessaoSalva = _repositorioSessaoWebSockets.ObterPorChave(usuario.ChaveAcesso);

                if (sessaoSalva != null)
                {
                    _repositorioSessaoWebSockets.Excluir(sessaoSalva.Id);
                }
                _repositorioSessaoWebSockets.Inserir(clienteWebSockets);

                return clienteWebSockets;
            }
        }

        public IList<SessaoClienteWebSockets> TodosClientesDisponivel()
        {
            return _repositorioSessaoWebSockets.Todos();
        }
    }
}
