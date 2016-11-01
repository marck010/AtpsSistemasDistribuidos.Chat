using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioBase
    {
        IList<T> Todos<T>() where T : EntidadeBase;
        T Obter<T>(int id, bool naoPermitirNulo = false) where T : EntidadeBase;
        void Inserir<T>(T entidade) where T : EntidadeBase;
        void Atualizar<T>(T entidade) where T : EntidadeBase;
        void Excluir<T>(int id) where T : EntidadeBase;
    }
}



