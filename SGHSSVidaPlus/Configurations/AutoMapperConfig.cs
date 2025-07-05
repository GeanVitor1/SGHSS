using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.MVC.Models;
using SGHSSVidaPlus.MVC.Data; // Adicionar este using para ApplicationUser
using static SGHSSVidaPlus.MVC.Areas.Identity.Pages.Account.LoginModel; // Importante para acessar RegisterPatientInputModel

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

            // Mapeamento Paciente (ViewModel para Entidade e vice-versa)
            CreateMap<Paciente, PacienteViewModel>().ReverseMap();

            // Mapeamento de PacienteViewModel para ApplicationUser
            CreateMap<PacienteViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Bloqueado, opt => opt.MapFrom(src => false));

            // Mapeamento de RegisterPatientInputModel para ApplicationUser (Corrigido na última interação)
            CreateMap<RegisterPatientInputModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Bloqueado, opt => opt.MapFrom(src => false));

            // NOVO MAPEAMENTO NECESSÁRIO: RegisterPatientInputModel para Paciente
            // Isso permite que o AutoMapper converta os dados de entrada do registro para a entidade Paciente.
            CreateMap<RegisterPatientInputModel, Paciente>()
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore()) // Será preenchido manualmente
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())             // Será preenchido manualmente
                .ForMember(dest => dest.UsuarioInclusao, opt => opt.Ignore())   // Será preenchido manualmente
                .ForMember(dest => dest.DataInclusao, opt => opt.Ignore())      // Será preenchido manualmente
                .ForMember(dest => dest.Contatos, opt => opt.Ignore())          // Contatos serão manipulados separadamente
                .ForMember(dest => dest.Historico, opt => opt.Ignore());        // Histórico será manipulado separadamente

            // Mapeamentos para outras ViewModels
            CreateMap<ProfissionalSaude, ProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<PacienteContato, PacienteContatoViewModel>().ReverseMap();
            CreateMap<HistoricoPaciente, HistoricoPacienteViewModel>().ReverseMap();
            CreateMap<FormacaoAcademicaProfissionalSaude, FormacaoAcademicaProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<CursosCertificacoesProfissionalSaude, CursosCertificacoesProfissionalSaudeViewModel>().ReverseMap();
            CreateMap<AgendamentoPaciente, AgendamentoPacienteViewModel>().ReverseMap();
        }
    }
}