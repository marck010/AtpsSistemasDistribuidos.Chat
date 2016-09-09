using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioBaseIntegrador<T>
    {
        IList<T> Todos();
        T Obter(object id);
        void Inserir(T entidade);
        void Atualizar(T entidade);
        void Excluir(object id);
    }
}



