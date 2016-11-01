using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.Excessoes;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioAtendente : RepositorioBaseIntegrador<Atendente>, IRepositorioAtendente
    {
        public Atendente ObterPorChave(string chave, bool naoPermitirNulo = false)
        {
            var atendente = Todos().FirstOrDefault(u => u.Usuario.ChaveAcesso== chave);

            if (atendente == null && naoPermitirNulo)
            {
                throw new ValidacaoException("Atendente não encontrado");
            }

            return atendente;
        }

        public Atendente ObterPorLogin(string login, bool naoPermitirNulo = false)
        {
            var atendente = Todos().FirstOrDefault(u => u.Usuario.Login == login);

            if (atendente == null && naoPermitirNulo)
            {
                throw new ValidacaoException("Atendente não encontrado");
            }

            return atendente;
        }
    }
}
