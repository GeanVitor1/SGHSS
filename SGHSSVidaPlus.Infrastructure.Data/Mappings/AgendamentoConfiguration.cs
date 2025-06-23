using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
    {
        public void Configure(EntityTypeBuilder<Agendamento> builder)
        {
            builder.Property(a => a.Descricao).IsRequired().HasColumnType("varchar(300)");
            builder.Property(a => a.UsuarioInclusao).IsRequired().HasColumnType("varchar(30)");
            builder.Property(a => a.UsuarioEncerramento).HasColumnType("varchar(30)");

            builder.HasOne(a => a.ProfissionalResponsavel)
                   .WithMany(ps => ps.AgendamentosRealizados)
                   .HasForeignKey(a => a.ProfissionalResponsavelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Agendamentos");
        }
    }
}