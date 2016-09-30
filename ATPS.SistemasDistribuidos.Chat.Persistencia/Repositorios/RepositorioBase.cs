using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.IOC;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioBaseIntegrador<T> : IRepositorioBaseIntegrador<T> where T : EntidadeBase
    {
        private IRepositorioBase _repositorioBase = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioBase>();

        public IList<T> Todos()
        {
            return _repositorioBase.Todos<T>();
        }

        public T Obter(int id)
        {
            return _repositorioBase.Obter<T>(id);
        }

        public void Inserir(T entidade)
        {
            entidade.Validar();
            _repositorioBase.Inserir<T>(entidade);
        }

        public void Atualizar(T entidade)
        {
            entidade.Validar();
            _repositorioBase.Atualizar<T>(entidade);
        }

        public void Excluir(int id)
        {
            _repositorioBase.Excluir<T>(id);
        }
    }
}
