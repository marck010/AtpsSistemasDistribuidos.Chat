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
        private readonly IUsuarioServico _servicoUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IUsuarioServico>();
        private readonly IRepositorioConversa _repositorioConversa = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioConversa>();

        public Conversa Enviar(string remetente, string destinatario, string textoMensagem, Guid? identificador)
        {
            var usuario = _servicoUsuario.ObterPorNome(remetente);

            if (usuario != null)
            {
                var conversa = identificador!=null?_repositorioConversa.Obter(identificador):null;
                var usuarioRemetente = _servicoUsuario.ObterPorNome(remetente);
                var usuarioDestinatario = _servicoUsuario.ObterPorNome(destinatario);

                var mensagem = new Mensagem
                {
                    Texto = textoMensagem,
                    DataEnvio = DateTime.Now,
                    Remetente = usuarioRemetente,
                    Destinatario = usuarioDestinatario
                };

                if (conversa != null)
                {
                    conversa.Mensagens.Add(mensagem);
                    _repositorioConversa.Atualizar(conversa);
                }
                else
                {
                    conversa = new Conversa() { Identificador = Guid.NewGuid() };
                    conversa.Mensagens.Add(mensagem);
                    usuario.Conversas.Add(conversa);
                    _repositorioConversa.Inserir(conversa);
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
