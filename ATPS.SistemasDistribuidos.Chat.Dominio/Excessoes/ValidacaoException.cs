using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Excessoes
{
    public class ValidacaoException : Exception
    {
        public ValidacaoException(string mensagem)
            : base(mensagem)
        {

        }
    }
}
