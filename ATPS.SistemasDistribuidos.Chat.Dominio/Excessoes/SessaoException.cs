using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Excessoes
{
    public class SessaoException : Exception
    {
        public SessaoException(string mensagem):base(mensagem)
        {

        }
    }
}
