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

        public void Registrar<TInterface, TEntidade>()
        {
            if (!ItemExiste(typeof(TInterface)))
            {
                ListaDependencias.Add(typeof(TInterface), typeof(TEntidade));
            }
            else
            {
                throw new Exception("Dependência já adicionada");
            }
        }

        public TInterface Resolver<TInterface>()
        {
            if (!ItemExiste(typeof(TInterface)))
            {
                var item = Obter(typeof(TInterface));
                return (TInterface)Activator.CreateInstance(item.Value);
            }

            throw new Exception("Não foi adicionada uma dependência para essa interface.");
        }

        private KeyValuePair<Type, Type> Obter(Type @interface)
        {
            return ListaDependencias.SingleOrDefault(x => x.Key == @interface.GetType());
        }

        private bool ItemExiste(Type item)
        {
            return Obter(item).Equals(default(KeyValuePair<Type, Type>));
        }
    }
}
