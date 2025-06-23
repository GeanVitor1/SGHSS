using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class AgendamentoPacienteConfiguration : IEntityTypeConfiguration<AgendamentoPaciente>
    {
        public void Configure(EntityTypeBuilder<AgendamentoPaciente> builder)
        {
            builder.HasKey(ap => ap.Id); // Garante que Id é a chave primária

            builder.HasOne(ap => ap.Agendamento)
                   .WithMany(a => a.PacientesAgendados)
                   .HasForeignKey(ap => ap.AgendamentoId)
                   .OnDelete(DeleteBehavior.Cascade); // Se um agendamento for excluído, a ligação também é.

            builder.HasOne(ap => ap.Paciente)
                   .WithMany(p => p.Agendamentos)
                   .HasForeignKey(ap => ap.PacienteId)
                   .OnDelete(DeleteBehavior.Cascade); // Se um paciente for excluído, suas ligações com agendamentos são também.

            builder.ToTable("AgendamentosPacientes");
        }
    }
}