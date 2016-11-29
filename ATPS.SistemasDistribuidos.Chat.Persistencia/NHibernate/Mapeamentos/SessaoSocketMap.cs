using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate.Mapeamentos
{
    public class SessaoClienteSocketMap : ClassMap<SessaoClienteSocket>
    {
        public SessaoClienteSocketMap()
        {
            Table("SessaoClienteSocket");

            Id(x => x.Id);

            Map(x => x.Chave)
                .Not.Nullable()
                .Length(50);

            References(x => x.Usuario)
                .Column("Usuario_Id")
                .ForeignKey("FK_Mensagem_Usuario_Id");
        }
    }
}
