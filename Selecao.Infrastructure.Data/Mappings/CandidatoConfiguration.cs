using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Selecao.Domain.Entities;

namespace Selecao.Infrastructure.Data.Mappings
{
    public class CandidatoConfiguration : IEntityTypeConfiguration<Candidato>
    {
        public void Configure(EntityTypeBuilder<Candidato> builder)
        {
            builder.Property(c => c.Nome).IsRequired().HasColumnType("varchar(200)");
            builder.Property(c => c.UsuarioInclusao).IsRequired().HasColumnType("varchar(30)");
            builder.Property(c => c.Logradouro).HasColumnType("varchar(300)");
            builder.Property(c => c.Bairro).HasColumnType("varchar(50)");

            builder.ToTable("Candidatos");
        }

        public class ContatoConfiguration : IEntityTypeConfiguration<CandidatoContato>
        {
            public void Configure(EntityTypeBuilder<CandidatoContato> builder)
            {
                builder.Property(c => c.Contato).IsRequired().HasColumnType("varchar(200)");

                builder.ToTable("CandidatosContatos");
            }
        }

        public void Configure(EntityTypeBuilder<CandidatoExperiencia> builder)
        {
            builder.Property(e => e.Cargo).IsRequired().HasColumnType("varchar(300)");

            builder.Property(e => e.Empregador).IsRequired().HasColumnType("varchar(100)");

            builder.Property(e => e.Descricao).HasColumnType("varchar(3000)");

            builder.ToTable("CandidatoExperiencias");
        }

        public void Configure(EntityTypeBuilder<CandidatoFormacao> builder)
        {
            builder.Property(f => f.Titulo).IsRequired().HasColumnType("varchar(1000)");
            builder.Property(f => f.Area).IsRequired().HasColumnType("varchar(100)");

            builder.Property(f => f.InstituicaoEnsino).HasColumnType("varchar(300)");

            builder.Property(f => f.Descricao).HasColumnType("varchar(3000)");

            builder.Property(f => f.AnoConclusao).HasColumnType("varchar(6)");

            builder.ToTable("CandidatoFormacoes");
        }

        public void Configure(EntityTypeBuilder<CandidatoCurso> builder)
        {
            builder.Property(c => c.Titulo).IsRequired().HasColumnType("varchar(1000)");

            builder.Property(c => c.InstituicaoEnsino).HasColumnType("varchar(300)");

            builder.Property(c => c.DuracaoHoras).HasColumnType("float");

            builder.Property(c => c.Descricao).HasColumnType("varchar(3000)");

            builder.Property(c => c.AnoConclusao).HasColumnType("varchar(6)");

            builder.Property(c => c.Area).IsRequired().HasColumnType("varchar(100)");

            builder.ToTable("CandidatoCursos");
        }
    }
}
