using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHSSVidaPlus.Domain.Entities;

namespace SGHSSVidaPlus.Infrastructure.Data.Mappings
{
    public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
    {
        public void Configure(EntityTypeBuilder<Paciente> builder)
        {
            builder.Property(p => p.Nome).IsRequired().HasColumnType("varchar(200)");
            builder.Property(p => p.CPF).IsRequired().HasColumnType("varchar(14)"); // Ex: "123.456.789-00"
            builder.Property(p => p.Endereco).HasColumnType("varchar(500)");
            builder.Property(p => p.EstadoCivil).HasColumnType("varchar(20)");
            builder.Property(p => p.UsuarioInclusao).IsRequired().HasColumnType("varchar(30)");

            builder.HasIndex(p => p.CPF).IsUnique(); // Garante que o CPF é único

            builder.ToTable("Pacientes");
        }
    }
}