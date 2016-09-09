
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioBaseMemoria : IRepositorioBase
    {
        private static List<EntidadeBase> Entidades { get; set; }

        public T Obter<T>(object id) where T : EntidadeBase
        {
            return (T)Entidades.SingleOrDefault(x => x.Id == id);
        }

        public IList<T> Todos<T>() where T : EntidadeBase
        {
            return Entidades.Cast<T>().ToList();
        }

        public void Inserir<T>(T entidade) where T : EntidadeBase
        {
            if (!ItemExiste<T>(entidade.Id))
            {
                if (entidade.Id == null)
                {
                    int ultimoId = Entidades.Max(x => (int)x.Id);
                    entidade.Id = ++ultimoId;
                }
                Entidades.Add(entidade);
            }
            else
            {
                throw new Exception("Item com o mesmo identificador já inserido.");
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
                throw new Exception(String.Format("Objeto com o identificador \"{0}\" não encontrado.", entidade.Id));
            }
        }

        public void Excluir<T>(object id) where T : EntidadeBase
        {
            if (ItemExiste<T>(id))
            {
                var item = Obter<T>(id);
                Entidades.Remove(item);
            }
            else
            {
                throw new Exception(String.Format("Item com o identificador \"{0}\" não encontrado.", id));
            }
        }

        private bool ItemExiste<T>(object identificador) where T : EntidadeBase
        {
            return Obter<T>(identificador) != null;
        }
    }
}
