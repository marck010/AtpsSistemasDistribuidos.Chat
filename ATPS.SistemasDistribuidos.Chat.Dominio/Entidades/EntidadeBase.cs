using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Dominio.Entidades
{
    public abstract class EntidadeBase
    {
        public virtual int Id { get; set; }

        public abstract void Validar();
    }
}
