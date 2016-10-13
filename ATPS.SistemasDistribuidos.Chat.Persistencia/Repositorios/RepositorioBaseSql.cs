
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.Excessoes;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioBaseSql : IRepositorioBase
    {
        private static List<EntidadeBase> Entidades { get; set; }

        public RepositorioBaseSql()
        {
        }

        static RepositorioBaseSql()
        {
            Entidades = new List<EntidadeBase>();
        }

        public T Obter<T>(int id) where T : EntidadeBase
        {
            return (T)Entidades.SingleOrDefault(x => x.GetType() == typeof(T) && x.Id.ToString() == id.ToString());
        }

        public IList<T> Todos<T>() where T : EntidadeBase
        {
            return Entidades.Where(x=>x.GetType() == typeof(T)).Cast<T>().ToList();
        }

        public void Inserir<T>(T entidade) where T : EntidadeBase
        {
            if (!ItemExiste<T>(entidade.Id))
            {
                if (entidade.Id == null)
                {
                    int ultimoId = Entidades.Any() ? Todos<T>().Max(x => (int)x.Id) : 0;
                    entidade.Id = ++ultimoId;
                }
                Entidades.Add(entidade);
            }
            else
            {
                throw new ValidacaoException("Item com o mesmo identificador já inserido.");
            }
        }

        public void Atualizar<T>(T entidade) where T : EntidadeBase
        {
            if (ItemExiste<T>(entidade.Id))
            {
                var entidadeSalva = Obter<T>(entidade.Id);
                Entidades.Remove((T)entidadeSalva);
                Entidades.Add((T)entidade);
            }
            else
            {
                throw new ValidacaoException(String.Format("Objeto com o identificador \"{0}\" não encontrado.", entidade.Id));
            }
        }

        public void Excluir<T>(int id) where T : EntidadeBase
        {
            if (ItemExiste<T>(id))
            {
                var item = Obter<T>(id);
                Entidades.Remove(item);
            }
            else
            {
                throw new ValidacaoException(String.Format("Item com o identificador \"{0}\" não encontrado.", id));
            }
        }

        private bool ItemExiste<T>(int id) where T : EntidadeBase
        {
            return Obter<T>(id) != null;
        }
    }
}
