using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class PacienteContatoConfiguration : IEntityTypeConfiguration<PacienteContato>
    {
        public void Configure(EntityTypeBuilder<PacienteContato> builder)
        {
            builder.Property(pc => pc.Contato).IsRequired().HasColumnType("varchar(200)");
            builder.Property(pc => pc.Tipo).HasColumnType("varchar(50)");

            builder.HasOne(pc => pc.Paciente)
                   .WithMany(p => p.Contatos)
                   .HasForeignKey(pc => pc.PacienteId);

            builder.ToTable("PacienteContatos");
        }
    }
}