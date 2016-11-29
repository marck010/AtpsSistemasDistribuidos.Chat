
using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATPS.SistemasDistribuidos.Chat.Dominio.Interfaces.Repositorios;
using ATPS.SistemasDistribuidos.Dominio.Excessoes;
using ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Linq;
using NHibernate.Impl;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.Repositorios
{
    public class RepositorioBaseSql : IRepositorioBase
    {
        private ISession _sessao = ProvedorSessao.ObterSessao();

        public T Obter<T>(int id, bool naoPermitirNulo = false) where T : EntidadeBase
        {
            var objeto = _sessao.Get<T>(id);

            if (naoPermitirNulo && objeto == null)
            {
                throw new ValidacaoException(String.Format("Objeto {0} não encontrado", typeof(T).ToString()));
            }

            return objeto;
        }

        protected virtual IQueryable<T> Query<T>() where T : EntidadeBase
        {
            var query = _sessao.Query<T>();
            return query;
        }

        public virtual IList<T> Todos<T>() where T : EntidadeBase
        {
            var query = Query<T>();
            return query.ToList();
        }

        public virtual void Inserir<T>(T obj) where T : EntidadeBase
        {
            using (var transacao = _sessao.BeginTransaction())
            {
                try
                {
                    _sessao.Save(obj);
                    transacao.Commit();
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                    throw ex;
                }
            }
        }

        public virtual void Atualizar<T>(T obj) where T : EntidadeBase
        {
            using (var transacao = _sessao.BeginTransaction())
            {
                try
                {
                    _sessao.Update(obj);
                    transacao.Commit();
                }
                catch (System.Exception ex)
                {
                    ((SessionImpl)_sessao).PersistenceContext.EntityEntries.Remove(obj);
                    transacao.Rollback();
                    throw ex;
                }
            }
        }

        public virtual void Excluir<T>(int obj) where T : EntidadeBase
        {
            using (var transacao = _sessao.BeginTransaction())
            {
                try
                {
                    _sessao.Delete(obj);
                    transacao.Commit();
                }
                catch (System.Exception ex)
                {
                    transacao.Rollback();
                    throw ex;
                }
            }
        }

        protected void VerificarNotNull<T>(bool notNull, T obj, int? id = null) where T : EntidadeBase
        {
            if (notNull && obj == null)
            {
                string mensagem = "Objeto não encontrado: " + typeof(T);
                if (id != null) mensagem += " / Chave primária: " + id;
                throw new ValidacaoException(mensagem);
            }
        }
    }
}
