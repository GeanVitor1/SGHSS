using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class CursosCertificacoesProfissionalSaudeConfiguration : IEntityTypeConfiguration<CursosCertificacoesProfissionalSaude>
    {
        public void Configure(EntityTypeBuilder<CursosCertificacoesProfissionalSaude> builder)
        {
            builder.Property(c => c.Titulo).IsRequired().HasColumnType("varchar(500)");
            builder.Property(c => c.InstituicaoEnsino).HasColumnType("varchar(300)");
            builder.Property(c => c.DuracaoHoras).IsRequired();
            builder.Property(c => c.AnoConclusao).HasColumnType("varchar(6)");
            builder.Property(c => c.Descricao).HasColumnType("varchar(2000)");

            builder.HasOne(c => c.ProfissionalSaude)
                   .WithMany(ps => ps.Cursos)
                   .HasForeignKey(c => c.ProfissionalSaudeId);

            builder.ToTable("CursosCertificacoesProfissionaisSaude");
        }
    }
}