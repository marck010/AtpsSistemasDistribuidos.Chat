using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate.Mapeamentos
{
    public class UsuarioMap : ClassMap<Usuario>
    {
        public UsuarioMap()
        {
            Table("Usuario");

            Id(x => x.Id);

            Map(x => x.Nome)
                .Not.Nullable()
                .Length(200);

            Map(x => x.Email)
                .Not.Nullable()
                .Length(200);

            Map(x => x.Telefone)
                .Nullable()
                .Length(15); ;

            Map(x => x.ChaveAcesso)
                .Nullable()
                .Length(50);

            Map(x => x.Login)
                .Not.Nullable()
                .Length(50);

            Map(x => x.Atendente)
                .Not.Nullable()
                .Length(50);

            References(x => x.SessaoSocketAtiva)
                .Column("SessaoSocketAtiva_Id")
                .ForeignKey("FK_Mensagem_SessaoSocketAtiva_Id");

            HasMany(x => x.Atendimentos)
                .KeyColumn("Cliente_Id")
              .Cascade.All()
              .Inverse();
        }
    }
}
