using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class HistoricoPacienteConfiguration : IEntityTypeConfiguration<HistoricoPaciente>
    {
        public void Configure(EntityTypeBuilder<HistoricoPaciente> builder)
        {
            builder.Property(hp => hp.Titulo).IsRequired().HasColumnType("varchar(200)"); // Título é Required
            // CORREÇÃO AQUI: NÃO USAR .IsRequired() para Descricao e ProfissionalResponsavel
            builder.Property(hp => hp.Descricao).HasColumnType("varchar(4000)");
            builder.Property(hp => hp.ProfissionalResponsavel).HasColumnType("varchar(100)");

            builder.HasOne(hp => hp.Paciente)
                   .WithMany(p => p.Historico)
                   .HasForeignKey(hp => hp.PacienteId);

            builder.ToTable("HistoricoPacientes");
        }
    }
}