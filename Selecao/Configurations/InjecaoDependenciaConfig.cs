using Microsoft.EntityFrameworkCore;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Domain.Interfaces.Service;
using Selecao.Domain.Service;
using Selecao.Infrastructure.Data.Context;
using Selecao.Infrastructure.Data.Repositories;


namespace Selecao.Configurations
{
    public static class InjecaoDependenciaConfig
    {
        public static IServiceCollection ResolverDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SelecaoContext>(options => options.UseSqlServer(configuration.GetConnectionString("Selecao")));


            //Injeções de Interface
            services.AddScoped<ICandidatosService, CandidatosService>();
            services.AddScoped<IEtapasSelecaoService, EtapasSelecaoService>();
            services.AddScoped<ISelecaoService, SelecaoService>();

            //Injecoes de Interface
            services.AddScoped<ICandidatosRepository, CandidatosRepository>();
            services.AddScoped<IEtapaSelecaoRepository, EtapasSelecaoRepository>();
            services.AddScoped<ISelecaoRepository, SelecaoRepository>();

            return services;
        }
    }
}
