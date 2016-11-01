using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using ATPS.SistemasDistribuidos.Dominio.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Dominio.Servicos
{
    public class ServicoAtendimento : IServicoAtendimento
    {
        private readonly IServicoUsuario _servicoUsuario = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoUsuario>();
        private readonly IServicoAtendente _servicoAtendente = ResolvedorDependenciaDominio.Instancia.Resolver<IServicoAtendente>();
        private readonly IRepositorioAtendimento _repositorioConversa = ResolvedorDependenciaDominio.Instancia.Resolver<IRepositorioAtendimento>();

        public Atendimento SalvarAtualizarAtendimento(string chaveAcessoRemetente, Usuario usuarioDestinatario, string textoMensagem, Atendimento atendimentoEmAndamento)
        {
            var usuarioRemetente = _servicoUsuario.ObterPorChave(chaveAcessoRemetente);

            if (usuarioRemetente != null)
            {
                var atendimento = atendimentoEmAndamento != null ? _repositorioConversa.Obter(atendimentoEmAndamento.Id) : null;

                var mensagem = new Mensagem(textoMensagem, usuarioRemetente, usuarioDestinatario);

                if (atendimento != null)
                {
                    atendimento.Mensagens.Add(mensagem);
                    _repositorioConversa.Atualizar(atendimento);
                }
                else
                {
                    var atendente = _servicoAtendente.ObterPorChaveAcesso(chaveAcessoRemetente);
                    atendimento = SalvarNovoAtendimento(usuarioDestinatario, atendimento, mensagem, atendente);

                    usuarioRemetente.Atendimentos.Add(atendimento);
                    usuarioDestinatario.Atendimentos.Add(atendimento);

                    _servicoUsuario.Atualizar(usuarioDestinatario.Id, usuarioDestinatario.Nome, usuarioDestinatario.Email, usuarioDestinatario.Telefone, disponivel: false);
                    _servicoUsuario.Atualizar(usuarioRemetente.Id, usuarioRemetente.Nome, usuarioRemetente.Email, usuarioRemetente.Telefone, disponivel: false);
                
                }

                return atendimento;
            }
            else
            {
                throw new ValidacaoException("Remetente não encontrado");
            }
        }

        private Atendimento SalvarNovoAtendimento(Usuario usuarioDestinatario, Atendimento atendimento, Mensagem mensagem, Atendente atendente)
        {

            atendimento = new Atendimento(usuarioDestinatario, atendente);
            atendimento.Mensagens.Add(mensagem);
            mensagem.Atendimento = atendimento;

            _repositorioConversa.Inserir(atendimento);
            return atendimento;
        }
    }
}
