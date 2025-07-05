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
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.MapFrom(src => src.ProfissionalResponsavel))
                .ForMember(dest => dest.Paciente, opt => opt.MapFrom(src => src.Paciente))
                .ForMember(dest => dest.Local, opt => opt.MapFrom(src => src.Local));

            // Mapeamento de ViewModel para Entidade (Usado em POSTs: Incluir, Editar)
            CreateMap<AgendamentoViewModel, Agendamento>()
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
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.Ignore())
                .ForMember(dest => dest.Paciente, opt => opt.Ignore())
                .ForMember(dest => dest.PacientesAgendados, opt => opt.Ignore())
                .ForMember(dest => dest.DataInclusao, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioInclusao, opt => opt.Ignore());

            // Mapeamentos para outras ViewModels (mantenha como estão, o ReverseMap aqui está ok)
            // ATUALIZADO: Adicionando mapeamento para UserId
            CreateMap<Paciente, PacienteViewModel>()
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // O email não vem da entidade Paciente diretamente, mas do ApplicationUser
                .ForMember(dest => dest.Senha, opt => opt.Ignore()) // A senha nunca deve ser mapeada para a ViewModel
                .ForMember(dest => dest.ConfirmarSenha, opt => opt.Ignore()) // A confirmação de senha também não
                .ForMember(dest => dest.TipoConsultaDesejada, opt => opt.Ignore()) // O tipo de consulta é apenas para o cadastro inicial do paciente
                .ReverseMap(); // Usamos ReverseMap para PacienteViewModel para criar e editar

            CreateMap<ProfissionalSaude, ProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<PacienteContato, PacienteContatoViewModel>().ReverseMap();
            CreateMap<HistoricoPaciente, HistoricoPacienteViewModel>().ReverseMap();
            CreateMap<FormacaoAcademicaProfissionalSaude, FormacaoAcademicaProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<CursosCertificacoesProfissionalSaude, CursosCertificacoesProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<AgendamentoPaciente, AgendamentoPacienteViewModel>().ReverseMap();
        }
    }
}