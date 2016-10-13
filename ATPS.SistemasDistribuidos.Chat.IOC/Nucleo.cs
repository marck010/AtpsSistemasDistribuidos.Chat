using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.IOC
{
    public class Nucleo
    {
        private Dictionary<Type, Type> ListaDependencias { get; set; }
        public Nucleo()
        {
            ListaDependencias = new Dictionary<Type, Type>();
        }

        public void Registrar<TInterface, TEntidade>()
        {
            if (!ItemExiste(typeof(TInterface)))
            {
                ListaDependencias.Add(typeof(TInterface), typeof(TEntidade));
            }
            else
            {
                throw new ValidacaoException(String.Format("Dependência para a interface {0} já adicionada", typeof(TInterface).ToString()));
            }
        }

        public TInterface Resolver<TInterface>()
        {
            if (ItemExiste(typeof(TInterface)))
            {
                var item = Obter(typeof(TInterface));
                return (TInterface)Activator.CreateInstance(item.Value);
            }

            throw new ValidacaoException(String.Format("Não foi adicionada uma dependência para a interface {0}.", typeof(TInterface).ToString()));
        }

        private KeyValuePair<Type, Type> Obter(Type @interface)
        {
            return ListaDependencias.SingleOrDefault(x => x.Key == @interface);
        }

        private bool ItemExiste(Type item)
        {
            return !Obter(item).Equals(default(KeyValuePair<Type, Type>));
        }
    }
}
