using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class ProfissionalSaudeConfiguration : IEntityTypeConfiguration<ProfissionalSaude>
    {
        public void Configure(EntityTypeBuilder<ProfissionalSaude> builder)
        {
            builder.Property(ps => ps.Nome).IsRequired().HasColumnType("varchar(200)");
            builder.Property(ps => ps.Cargo).IsRequired().HasColumnType("varchar(100)");

            // Estas propriedades não são IsRequired()
            builder.Property(ps => ps.EspecialidadeCargo).HasColumnType("varchar(100)");
            builder.Property(ps => ps.Telefone).HasColumnType("varchar(20)");
            builder.Property(ps => ps.Email).HasColumnType("varchar(100)");
            builder.Property(ps => ps.RegistroConselho).HasColumnType("varchar(50)");
            builder.Property(ps => ps.UsuarioInclusao).HasColumnType("varchar(30)");

            builder.ToTable("ProfissionaisSaude");
        }
    }
}