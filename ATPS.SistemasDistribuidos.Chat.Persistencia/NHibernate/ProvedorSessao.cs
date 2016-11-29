using ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate.Mapeamentos;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate
{
    public class ProvedorSessao
    {
        public static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                           .Database(SQLiteConfiguration.Standard.UsingFile("Chat.db"))
                           .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMap>())
                           .CurrentSessionContext<WebSessionContext>()
                           .ExposeConfiguration(BuildSchema)
                           .BuildSessionFactory();
        }

        public static void BuildSchema(Configuration config)
        {
            var path = obterPath();

            if (!File.Exists(path + "Chat.db"))
            {
                File.Create(path + "Chat.db");

                new SchemaExport(config).Create(false, true);
            }
        }

        private static string obterPath()
        {
            string serverPath = HttpContext.Current.Server.MapPath("~/");
            return serverPath;
        }

        public static ISession ObterSessao()
        {
            return CreateSessionFactory().OpenSession();
        }
    }
}
