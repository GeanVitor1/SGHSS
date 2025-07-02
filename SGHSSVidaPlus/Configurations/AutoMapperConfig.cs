using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.MVC.Models;

namespace SGHSSVidaPlus.MVC.Configurations
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
            // Mapeamento de Entidade para ViewModel (Usado em GETs: Index, Editar, Visualizar)
            CreateMap<Agendamento, AgendamentoViewModel>()
                // Mapeia o nome do profissional (se existir)
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.MapFrom(src => src.ProfissionalResponsavel))
                // Mapeia o nome do paciente (se existir)
                .ForMember(dest => dest.Paciente, opt => opt.MapFrom(src => src.Paciente))
                // Mapeia Local para LocalDeAtendimento na ViewModel (se o nome da prop for diferente)
                .ForMember(dest => dest.Local, opt => opt.MapFrom(src => src.Local));

            // Mapeamento de ViewModel para Entidade (Usado em POSTs: Incluir, Editar)
            CreateMap<AgendamentoViewModel, Agendamento>()
                // Mapear propriedades diretas
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.Observacoes, opt => opt.MapFrom(src => src.Observacoes))
                .ForMember(dest => dest.Local, opt => opt.MapFrom(src => src.Local))
                .ForMember(dest => dest.DataHoraAgendamento, opt => opt.MapFrom(src => src.DataHoraAgendamento))
                .ForMember(dest => dest.PacienteId, opt => opt.MapFrom(src => src.PacienteId))
                .ForMember(dest => dest.ProfissionalResponsavelId, opt => opt.MapFrom(src => src.ProfissionalResponsavelId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Encerrado, opt => opt.MapFrom(src => src.Encerrado))
                .ForMember(dest => dest.UsuarioEncerramento, opt => opt.MapFrom(src => src.UsuarioEncerramento))
                .ForMember(dest => dest.DataEncerramento, opt => opt.MapFrom(src => src.DataEncerramento))

                // IGNORAR propriedades de navegação no mapeamento de ViewModel para Entidade
                // Elas não devem ser criadas ou atualizadas a partir da ViewModel, apenas o ID é suficiente
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.Ignore())
                .ForMember(dest => dest.Paciente, opt => opt.Ignore())
                // IGNORAR coleções aninhadas que não são tratadas diretamente pela ViewModel no POST principal
                .ForMember(dest => dest.PacientesAgendados, opt => opt.Ignore())
                .ForMember(dest => dest.TiposAtendimento, opt => opt.Ignore())
                // IGNORAR campos de auditoria que são preenchidos no serviço/repositório
                .ForMember(dest => dest.DataInclusao, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioInclusao, opt => opt.Ignore());



            // Mapeamentos para outras ViewModels (mantenha como estão, o ReverseMap aqui está ok)
            CreateMap<Paciente, PacienteViewModel>().ReverseMap();
            CreateMap<ProfissionalSaude, ProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<TipoAtendimento, TipoAtendimentoViewModel>().ReverseMap();
            CreateMap<PacienteContato, PacienteContatoViewModel>().ReverseMap();
            CreateMap<HistoricoPaciente, HistoricoPacienteViewModel>().ReverseMap();
            CreateMap<FormacaoAcademicaProfissionalSaude, FormacaoAcademicaProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<CursosCertificacoesProfissionalSaude, CursosCertificacoesProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<AgendamentoPaciente, AgendamentoPacienteViewModel>().ReverseMap();
            CreateMap<AgendamentoTipoAtendimento, AgendamentoTipoAtendimentoViewModel>().ReverseMap();
        }
    }
}