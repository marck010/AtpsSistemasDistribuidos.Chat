using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioAtendente : RepositorioBaseIntegrador<Atendente>, IRepositorioAtendente
    {
        public Atendente ObterPorChave(string chave)
        {
            var atendente = Todos().FirstOrDefault(u => u.Usuario.ChaveAcesso== chave);
            return atendente;
        }

        public Atendente ObterPorLogin(string login)
        {
            var atendente = Todos().FirstOrDefault(u => u.Usuario.Login == login);
            return atendente;
        }
    }
}
