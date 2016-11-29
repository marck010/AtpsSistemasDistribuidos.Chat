using ATPS.SistemasDistribuidos.Chat.Dominio.Entidades;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPS.SistemasDistribuidos.Chat.Persistencia.NHibernate.Mapeamentos
{
        public class AtendenteMap : ClassMap<Atendente>
        {
            public AtendenteMap()
            {
                Table("Atendente");

                Id(x => x.Id);
                
                Map(x=>x.Administrador)
                    .Not.Nullable();

                Map(x=>x.Senha)
                    .Not.Nullable();

                References(x=>x.Usuario)
                    .Column("Usuario_Id")
                    .ForeignKey("FK_Mensagem_Usuario_Id");
            }
        }
}
