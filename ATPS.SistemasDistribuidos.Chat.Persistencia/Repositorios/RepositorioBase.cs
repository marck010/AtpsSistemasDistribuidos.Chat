using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Chat.Dependencias;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioBaseIntegrador<T> : IRepositorioBaseIntegrador<T> where T : EntidadeBase
    {
        private IRepositorioBase _repositorioBase = ResolvedorDependencia.Instancia.Resolver<IRepositorioBase>();

        public IList<T> Todos()
        {
            return _repositorioBase.Todos<T>();
        }

        public T Obter(object id) {
           return _repositorioBase.Obter<T>(id);
        }

        public void Inserir(T entidade) 
        {
            _repositorioBase.Inserir<T>(entidade);
        }

        public void Atualizar(T entidade) 
        {
            _repositorioBase.Atualizar<T>(entidade);
        }
        
        public void Excluir(object id)
        {
            _repositorioBase.Excluir<T>(id);
        }
    }
}
