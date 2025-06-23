using AutoMapper;
using Microsoft.Extensions.DependencyInjection; // Necessário para IServiceCollection
using SGHSSVidaPlus.Domain.Entities; // <-- NAMESPACE ATUALIZADO PARA SUAS ENTIDADES
using SGHSSVidaPlus.MVC.Models; // <-- NAMESPACE ATUALIZADO PARA SEUS VIEWMARCOS (que você vai criar)

namespace SGHSSVidaPlus.MVC.Configurations // <-- NAMESPACE ATUALIZADO DA CONFIGURAÇÃO
{
    public static class AutoMapperConfig
    {
        public static IServiceCollection ConfigurarAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapping()); });
            var mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddMemoryCache(); // Mantido se você usa cache de memória

            return services;
        }
    }

    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            // Mapeamentos para suas NOVAS entidades e seus ViewModels
            CreateMap<Paciente, PacienteViewModel>().ReverseMap();
            CreateMap<ProfissionalSaude, ProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<Agendamento, AgendamentoViewModel>().ReverseMap();
            CreateMap<TipoAtendimento, TipoAtendimentoViewModel>().ReverseMap();

            // Mapeamentos para entidades auxiliares, se forem usadas diretamente em ViewModels
            CreateMap<PacienteContato, PacienteContatoViewModel>().ReverseMap();
            CreateMap<HistoricoPaciente, HistoricoPacienteViewModel>().ReverseMap();
            CreateMap<FormacaoAcademicaProfissionalSaude, FormacaoAcademicaProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<CursosCertificacoesProfissionalSaude, CursosCertificacoesProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<AgendamentoPaciente, AgendamentoPacienteViewModel>().ReverseMap();
            CreateMap<AgendamentoTipoAtendimento, AgendamentoTipoAtendimentoViewModel>().ReverseMap();
        }
    }
}