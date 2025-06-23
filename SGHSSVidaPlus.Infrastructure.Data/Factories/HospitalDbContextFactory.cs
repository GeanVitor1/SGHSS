using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System; // Necessário para Environment
using System.IO; // Necessário para Path.Combine, Directory

using SGHSSVidaPlus.Infrastructure.Data.Context;

namespace SGHSSVidaPlus.Infrastructure.Data
{
    public class HospitalDbContextFactory : IDesignTimeDbContextFactory<HospitalDbContext>
    {
        public HospitalDbContext CreateDbContext(string[] args)
        {
            // Este é o caminho correto para encontrar o appsettings.json no projeto de startup (SGHSSVidaPlus)
            // Ele navega do diretório atual (que é SGHSSVidaPlus.Infrastructure.Data)
            // para cima um nível (chegando na raiz da solução Selecao1),
            // e depois para baixo para a pasta do projeto SGHSSVidaPlus (o MVC).
            var startupProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SGHSSVidaPlus");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(startupProjectPath) // <-- CORREÇÃO AQUI
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // Adiciona o appsettings.json.Development, se existir
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Verifica se a string de conexão foi encontrada
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Erro: String de conexão 'DefaultConnection' não encontrada ou vazia no appsettings.json do projeto de startup ('SGHSSVidaPlus').");
            }

            var optionsBuilder = new DbContextOptionsBuilder<HospitalDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new HospitalDbContext(optionsBuilder.Options);
        }
    }
}