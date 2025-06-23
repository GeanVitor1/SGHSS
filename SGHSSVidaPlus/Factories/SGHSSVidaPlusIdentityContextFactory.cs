using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design; // Necessário para IDesignTimeDbContextFactory
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration e ConfigurationBuilder
using System.IO; // Necessário para Directory.GetCurrentDirectory()
using SGHSSVidaPlus.MVC.Data; // Para o seu SGHSSVidaPlusIdentityContext

namespace SGHSSVidaPlus.MVC // O namespace da sua camada MVC
{
    // Esta classe diz ao Entity Framework Core como criar uma instância do seu DbContext
    // para operações de design-time (como Add-Migration, Update-Database).
    public class SGHSSVidaPlusIdentityContextFactory : IDesignTimeDbContextFactory<SGHSSVidaPlusIdentityContext>
    {
        public SGHSSVidaPlusIdentityContext CreateDbContext(string[] args)
        {
            // Constrói a configuração para ler a string de conexão do appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Obtém a string de conexão
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Cria as opções do DbContext
            var optionsBuilder = new DbContextOptionsBuilder<SGHSSVidaPlusIdentityContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Retorna uma nova instância do seu DbContext de Identity
            return new SGHSSVidaPlusIdentityContext(optionsBuilder.Options);
        }
    }
}