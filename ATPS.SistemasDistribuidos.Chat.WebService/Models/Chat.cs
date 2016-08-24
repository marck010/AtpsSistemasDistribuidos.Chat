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
            Conversas = new List<ConversaModel>();
        }
        public List<ConversaModel> Conversas { get; set; }

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
        public ConversaModel Enviar(string remetente, string destinatario, string mensagem)
        {
            var conversa = Conversas.FirstOrDefault(conversaSalva => conversaSalva.Remetente.Nome == remetente && conversaSalva.Remetente.Nome == destinatario);
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
            }
            return conversa;
        }

    }
}