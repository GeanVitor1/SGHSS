using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class FormacaoAcademicaProfissionalSaudeConfiguration : IEntityTypeConfiguration<FormacaoAcademicaProfissionalSaude>
    {
        public void Configure(EntityTypeBuilder<FormacaoAcademicaProfissionalSaude> builder)
        {
            builder.Property(f => f.Titulo).IsRequired().HasColumnType("varchar(500)");
            builder.Property(f => f.InstituicaoEnsino).HasColumnType("varchar(300)");
            builder.Property(f => f.AnoConclusao).HasColumnType("varchar(6)");
            builder.Property(f => f.Descricao).HasColumnType("varchar(2000)");

            builder.HasOne(f => f.ProfissionalSaude)
                   .WithMany(ps => ps.Formacao)
                   .HasForeignKey(f => f.ProfissionalSaudeId);

            builder.ToTable("FormacoesAcademicasProfissionalSaude");
        }
    }
}