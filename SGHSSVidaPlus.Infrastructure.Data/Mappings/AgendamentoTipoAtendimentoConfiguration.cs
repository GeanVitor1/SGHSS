using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class AgendamentoTipoAtendimentoConfiguration : IEntityTypeConfiguration<AgendamentoTipoAtendimento>
    {
        public void Configure(EntityTypeBuilder<AgendamentoTipoAtendimento> builder)
        {
            builder.HasKey(ata => ata.Id); // Garante que Id é a chave primária

            builder.HasOne(ata => ata.Agendamento)
                   .WithMany(a => a.TiposAtendimento)
                   .HasForeignKey(ata => ata.AgendamentoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ata => ata.TipoAtendimento)
                   .WithMany(ta => ta.AgendamentosAssociados)
                   .HasForeignKey(ata => ata.TipoAtendimentoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("AgendamentosTiposAtendimento");
        }
    }
}