using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate.Mapeamentos
{
    public class AtendimentoMap : ClassMap<Atendimento>
    {
        public AtendimentoMap()
        {
            Table("Usuario");

            Id(x => x.Id);

            References(x => x.Cliente)
                .Column("Cliente_Id")
                .ForeignKey("FK_Mensagem_Cliente_Id");

            References(x => x.Atendente)
                .Column("Atendente_Id")
                .ForeignKey("FK_Mensagem_Atendente_Id");

            HasMany(x => x.Mensagens)
                .KeyColumn("Atendimento_Id")
                .Cascade.All()
                .Inverse();
        }
    }
}
