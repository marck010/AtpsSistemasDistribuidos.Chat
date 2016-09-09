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
    public class ConversaServico : IConversaServico
    {
        private readonly UsuarioServico _servicoUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<UsuarioServico>();
        private readonly IRepositorioConversa _repositorioConversa = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioConversa>();

        public Conversa Enviar(string remetente, string destinatario, string mensagem, Guid identificador)
        {
            var usuario = _servicoUsuario.ObterPorNome(remetente);

            if (usuario != null)
            {
                var conversa = _repositorioConversa.Obter(identificador);
                var remetenteObj = _servicoUsuario.ObterPorNome(remetente);
                var destinatarioObj = _servicoUsuario.ObterPorNome(destinatario);
                
                var mensagemObj = new Mensagem { Texto = mensagem, DataEnvio = DateTime.Now, Remetente = remetenteObj, Destinatario = destinatarioObj };
               
                if (conversa != null)
                {
                    conversa.Mensagens.Add(mensagemObj);
                    _repositorioConversa.Inserir(conversa);
                }
                else
                {
                    conversa = new Conversa(){Identificador = Guid.NewGuid()};
                    conversa.Mensagens.Add(mensagemObj);
                    usuario.Conversas.Add(conversa);
                    _repositorioConversa.Atualizar(conversa);
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
