using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate.Mapeamentos
{
    public class MensagemMap : ClassMap<Mensagem>
    {
        public MensagemMap()
        {
            Table("Mensagem");

            Id(x => x.Id);

            Map(x => x.Texto)
                .Not.Nullable()
                .Length(1000);

            Map(x => x.DataEnvio)
                .Not.Nullable();

            References(x => x.Remetente)
                .Column("Remetente_Id")
                .ForeignKey("FK_Mensagem_Rementente_Id");

            References(x => x.Destinatario)
                .Column("Destinatario_Id")
                .ForeignKey("FK_Mensagem_Destinatario_Id");

            References(x => x.Atendimento)
                .Column("Atendimento_Id")
                .ForeignKey("FK_Mensagem_Atendimento_Id");

        }
    }
}
