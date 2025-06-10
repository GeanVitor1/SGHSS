using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Selecao.Domain.Entities;

namespace Selecao.Infrastructure.Data.Context
{
    public class SelecaoContext : DbContext
    {
        public string StringConection { get; } = string.Empty;
        public SelecaoContext(DbContextOptions<SelecaoContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;

            var sqlServerOptionsExtensions = options.FindExtension<SqlServerOptionsExtension>();
            if (sqlServerOptionsExtensions != null)
                StringConection = sqlServerOptionsExtensions.ConnectionString;
        }

        //Coloque aqui suas entidades para mapeamento no contexto
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<CandidatoExperiencia> CandidatoExperiencias { get; set; }
        public DbSet<CandidatoFormacao> CandidatoFormacoes { get; set; }
        public DbSet<CandidatoCurso> CandidatoCursos { get; set; }
        public DbSet<CandidatoContato> CandidatoContatos { get; set; }
        public DbSet<EtapaSelecao> EtapasSelecao { get; set; }
        public DbSet<SelecaoCandidato> Selecao{ get; set; }
        public DbSet<CandidatoSelecaoCandidato> CandidatoSelecao { get; set; }
        public DbSet<EtapaSelecaoCandidato> EtapaSelecao { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SelecaoContext).Assembly);

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

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
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
