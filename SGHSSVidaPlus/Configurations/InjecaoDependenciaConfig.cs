using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration
using Microsoft.Extensions.DependencyInjection; // Necessário para IServiceCollection
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Usings atualizados
using SGHSSVidaPlus.Domain.Interfaces.Service; // Usings atualizados
using SGHSSVidaPlus.Infrastructure.Data.Context; // Usings atualizados
using SGHSSVidaPlus.Infrastructure.Data.Repositories; // Usings atualizados
using SGHSSVidaPlus.Application.Services; // Usings atualizados (onde estão as implementações dos serviços)


namespace SGHSSVidaPlus.MVC.Configurations // Namespace atualizado
{
    public static class InjecaoDependenciaConfig
    {
        public static IServiceCollection ResolverDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            // ATENÇÃO: Se esta linha for mantida aqui, remova-a do Program.cs
            services.AddDbContext<HospitalDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); // String de conexão atualizada


            // Injeções de Interfaces de Serviço (Business Logic)
            services.AddScoped<IPacienteService, PacienteService>(); // Atualizado de CandidatosService
            services.AddScoped<ITipoAtendimentoService, TipoAtendimentoService>(); // Atualizado de EtapasSelecaoService
            services.AddScoped<IAgendamentoService, AgendamentoService>(); // Atualizado de SelecaoService
            services.AddScoped<IProfissionalSaudeService, ProfissionalSaudeService>(); // ADICIONADO: serviço para ProfissionalSaude

            // Injeções de Interfaces de Repositório (Data Access)
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>)); // Repositório base genérico
            services.AddScoped<IPacienteRepository, PacienteRepository>();
            services.AddScoped<ITipoAtendimentoRepository, TipoAtendimentoRepository>();
            services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
            services.AddScoped<IProfissionalSaudeRepository, ProfissionalSaudeRepository>(); // ADICIONADO: repositório para ProfissionalSaude

            return services;
        }
    }
}