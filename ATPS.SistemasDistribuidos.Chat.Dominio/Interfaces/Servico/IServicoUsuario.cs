using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Servicos
{
    public interface IServicoUsuario
    {
        IList<Usuario> UsuariosAguardandoAtendimento();
        IList<Usuario> AtendentesDisponiveis();

        Usuario ObterPorChave(string chave, bool naoPermitirNulo = false);
        Usuario ObterPorLogin(string login, bool naoPermitirNulo = false);
        Usuario ObterPorNome(string nome, bool naoPermitirNulo = false);
        
        Usuario ConectarUsuario(string remetente, string chave);
        Usuario Inserir(string nome, string email, string telefone);
        Usuario Atualizar(int id, string nome, string email, string telefone, bool disponivel);
        void RemoverSessaoDoUsuario(Usuario usuario);
    }
}
