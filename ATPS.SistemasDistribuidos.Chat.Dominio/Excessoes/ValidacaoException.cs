using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Excessoes
{
    public class ValidacaoException : Exception
    {
        public List<String> ListaMensagens { get; set; }

        public ValidacaoException(string mensgem):base(mensgem)
        {
            ListaMensagens = new List<string>();
            ListaMensagens.Add(mensgem);
        }

        public ValidacaoException(List<string> mensagens) : base(String.Join(Environment.NewLine, mensagens))
        {
            ListaMensagens = mensagens;
        }
    }
}
