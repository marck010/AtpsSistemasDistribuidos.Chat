using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATPS.SistemasDistribuidos.Chat.WebService.Models
{
    public class Chat
    {
        private static object _lock = new object();
        private static Chat _instacia;
        private Chat()
        {
            UsuariosConectados = new List<UsuarioModel>();
        }

        public static List<UsuarioModel> UsuariosConectados { get; set; }

        public static Chat Instancia
        {
            get
            {
                if (_instacia == null)
                {
                    lock (_lock)
                    {
                        _instacia = new Chat();
                    }
                }
                return _instacia;
            }
        }

        public void ConectarUsuario(string remetente, string chave)
        {
            var usuario = UsuariosConectados.FirstOrDefault(u => u.Nome == remetente);

            if (usuario == null)
            {
                UsuariosConectados.Add(new UsuarioModel { Nome = remetente, ChaveConexao = chave });
            }
            else
            {
                if (usuario.ChaveConexao != chave)
                {
                    usuario.ChaveConexao = chave;
                }
      
            }
        }
        
        public UsuarioModel ObterUsuarioPorChave(string chave)
        { 
            var usuario = UsuariosConectados.FirstOrDefault(u => u.ChaveConexao == chave);

            return usuario;
        }

        public UsuarioModel ObterUsuarioPorNome(string nome)
        {
            var usuario = UsuariosConectados.FirstOrDefault(u => u.Nome == nome);

            return usuario;
        }

        public ConversaModel Enviar(string remetente, string destinatario, string mensagem)
        {
            var usuario = UsuariosConectados.FirstOrDefault(u => u.Nome == remetente);
            if (usuario!=null)
            {
                var conversa = usuario.Conversas.FirstOrDefault(conversaSalva => conversaSalva.Destinatario.Nome == destinatario && conversaSalva.Remetente.Nome == remetente);
                if (conversa != null)
                {
                    conversa.Mensagens.Add(new MensagemModel { Texto = mensagem, DataEnvio = DateTime.Now });
                }
                else
                {
                    conversa = new ConversaModel()
                    {
                        Destinatario = new UsuarioModel { Nome = destinatario },
                        Remetente = new UsuarioModel { Nome = remetente }
                    };
                    conversa.Mensagens.Add(new MensagemModel { Texto = mensagem, DataEnvio = DateTime.Now, Conversa = conversa });
                    usuario.Conversas.Add(conversa);
                }
                return conversa;
            }
            else
            {
                throw new Exception("Usuario não encontrado");
            }

        }

    }
}