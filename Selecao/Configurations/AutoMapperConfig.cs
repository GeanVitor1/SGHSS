using AutoMapper;
using Selecao.Domain.Entities;
using Selecao.MVC.Models;

namespace Selecao.MVC.Configurations
{
    public static class AutoMapperConfig
    {
        public static IServiceCollection ConfigurarAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapping()); });
            var mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddMemoryCache();

            return services;
        }
    }

    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Candidato, CandidatoViewModel>().ReverseMap();
            CreateMap<EtapaSelecao, EtapaSelecaoViewModel>().ReverseMap();
            CreateMap<SelecaoCandidato, SelecaoCandidatoViewModel>().ReverseMap();
        }
    }
}