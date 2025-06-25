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
            builder.Property(p => p.CPF).IsRequired().HasColumnType("varchar(14)");
            builder.Property(p => p.Endereco).IsRequired().HasColumnType("varchar(500)"); // Assumindo Endereco é Required
            builder.Property(p => p.EstadoCivil).IsRequired().HasColumnType("varchar(20)"); // Assumindo EstadoCivil é Required

            // CORREÇÃO AQUI: REMOVIDO .IsRequired() de UsuarioInclusao
            builder.Property(p => p.UsuarioInclusao).HasColumnType("varchar(30)");

            builder.Property(p => p.DataNascimento).IsRequired(); // DataNascimento é Required (tipo DateTime? agora)

            builder.HasIndex(p => p.CPF).IsUnique();

            builder.ToTable("Pacientes");
        }
    }
}