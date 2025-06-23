using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using SGHSSVidaPlus.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace SGHSSVidaPlus.Infrastructure.Data.Context
{
    public class HospitalDbContext : DbContext
    {
        public string StringConection { get; } = string.Empty;

        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;

            var sqlServerOptionsExtensions = options.FindExtension<SqlServerOptionsExtension>();
            if (sqlServerOptionsExtensions != null)
                StringConection = sqlServerOptionsExtensions.ConnectionString;
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<PacienteContato> PacienteContatos { get; set; }
        public DbSet<HistoricoPaciente> HistoricoPacientes { get; set; }

        public DbSet<ProfissionalSaude> ProfissionaisSaude { get; set; }
        public DbSet<FormacaoAcademicaProfissionalSaude> FormacoesAcademicasProfissionaisSaude { get; set; }
        public DbSet<CursosCertificacoesProfissionalSaude> CursosCertificacoesProfissionaisSaude { get; set; }

        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<AgendamentoPaciente> AgendamentosPacientes { get; set; }

        public DbSet<TipoAtendimento> TiposAtendimento { get; set; }
        public DbSet<AgendamentoTipoAtendimento> AgendamentosTiposAtendimento { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var navigations = entityType.GetNavigations();

                foreach (var navigation in navigations)
                {
                    var inverseNavigation = navigation.Inverse;

                    if (inverseNavigation != null)
                        modelBuilder.Entity(entityType.ClrType).Navigation(navigation.Name).AutoInclude(false);
                }
            }

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.CPF)
                .IsUnique();

            modelBuilder.Entity<AgendamentoPaciente>()
                .HasOne(ap => ap.Agendamento)
                .WithMany(a => a.PacientesAgendados)
                .HasForeignKey(ap => ap.AgendamentoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AgendamentoPaciente>()
                .HasOne(ap => ap.Paciente)
                .WithMany(p => p.Agendamentos)
                .HasForeignKey(ap => ap.PacienteId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.ProfissionalResponsavel)
                .WithMany(ps => ps.AgendamentosRealizados)
                .HasForeignKey(a => a.ProfissionalResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataInclusao") != null))
            {
                if (entry.State == EntityState.Added)
                    entry.Property("DataInclusao").CurrentValue = DateTime.Now;

                if (entry.State == EntityState.Modified)
                    entry.Property("DataInclusao").IsModified = false;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}