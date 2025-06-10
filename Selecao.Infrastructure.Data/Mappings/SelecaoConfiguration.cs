using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Selecao.Domain.Entities;

namespace Selecao.Infrastructure.Data.Mappings
{
    public class SelecaoConfiguration : IEntityTypeConfiguration<SelecaoCandidato>
    {
        public void Configure(EntityTypeBuilder<SelecaoCandidato> builder)
        {
            builder.Property(c => c.Cargo).IsRequired().HasColumnType("varchar(200)");
            builder.Property(c => c.UsuarioInclusao).IsRequired().HasColumnType("varchar(30)");
            builder.Property(c => c.UsuarioEncerramento).HasColumnType("varchar(30)");
            builder.Property(c => c.Objetivo).HasColumnType("varchar(3000)");

            builder.ToTable("Selecoes");
        }
    }
}
