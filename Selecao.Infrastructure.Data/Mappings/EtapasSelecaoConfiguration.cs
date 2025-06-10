using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Selecao.Domain.Entities;

namespace Selecao.Infrastructure.Data.Mappings
{
    public class EtapasSelecaoConfiguration : IEntityTypeConfiguration<EtapaSelecao>
    {
        public void Configure(EntityTypeBuilder<EtapaSelecao> builder)
        {
            builder.Property(e => e.Nome).IsRequired().HasColumnType("varchar(200)");
            builder.Property(e => e.UsuarioInclusao).IsRequired().HasColumnType("varchar(30)");

            builder.ToTable("EtapasSelecao");
        }
    }
}
