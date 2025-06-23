using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class TipoAtendimentoConfiguration : IEntityTypeConfiguration<TipoAtendimento>
    {
        public void Configure(EntityTypeBuilder<TipoAtendimento> builder)
        {
            builder.Property(ta => ta.Nome).IsRequired().HasColumnType("varchar(200)");
            builder.Property(ta => ta.UsuarioInclusao).IsRequired().HasColumnType("varchar(30)");

            builder.ToTable("TiposAtendimento");
        }
    }
}