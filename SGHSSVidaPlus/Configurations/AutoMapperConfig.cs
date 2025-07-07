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
                .ForMember(dest => dest.ProfissionalResponsavelId, opt => opt.MapFrom(src => src.ProfissionalResponsavelId))
                .ForMember(dest => dest.ProfissionalResponsavelNome, opt => opt.MapFrom(src => src.ProfissionalResponsavel != null ? src.ProfissionalResponsavel.Nome : null))
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.MapFrom(src => src.ProfissionalResponsavel)) // Mapeia o objeto ProfissionalSaude (entidade) para ProfissionalSaudeViewModel
                .ForMember(dest => dest.PacienteId, opt => opt.MapFrom(src => src.PacienteId))
                .ForMember(dest => dest.PacienteNome, opt => opt.MapFrom(src => src.Paciente != null ? src.Paciente.Nome : null))
                .ForMember(dest => dest.Paciente, opt => opt.MapFrom(src => src.Paciente)); // Mapeia o objeto Paciente (entidade) para PacienteViewModel

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
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.Ignore()) // Ignora o objeto de navegação ao mapear de ViewModel para Entidade
                .ForMember(dest => dest.Paciente, opt => opt.Ignore()) // Ignora o objeto de navegação ao mapear de ViewModel para Entidade
                .ForMember(dest => dest.PacientesAgendados, opt => opt.Ignore())
                .ForMember(dest => dest.DataInclusao, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioInclusao, opt => opt.Ignore());

            // Mapeamento Paciente (Entidade para ViewModel e vice-versa)
            CreateMap<Paciente, PacienteViewModel>().ReverseMap();

            // Mapeamento para PacienteAdminRegisterViewModel (específico para cadastro via admin)
            // Como PacienteAdminRegisterViewModel herda de PacienteViewModel, este mapeamento cobre ambos.
            CreateMap<Paciente, PacienteAdminRegisterViewModel>().ReverseMap();

            // Mapeamento de PacienteViewModel para ApplicationUser (para auto-cadastro ou atualização de usuário existente)
            CreateMap<PacienteViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Bloqueado, opt => opt.MapFrom(src => false));

            // Mapeamento de RegisterPatientInputModel para ApplicationUser (para auto-cadastro)
            CreateMap<RegisterPatientInputModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Bloqueado, opt => opt.MapFrom(src => false));

            // Mapeamento de RegisterPatientInputModel para Paciente (para auto-cadastro)
            CreateMap<RegisterPatientInputModel, Paciente>()
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioInclusao, opt => opt.Ignore())
                .ForMember(dest => dest.DataInclusao, opt => opt.Ignore())
                .ForMember(dest => dest.Contatos, opt => opt.Ignore())
                .ForMember(dest => dest.Historico, opt => opt.Ignore());

            // Mapeamento para ProfissionalSaude (Entidade para ViewModel e vice-versa)
            CreateMap<ProfissionalSaude, ProfissionalSaudeViewModel>().ReverseMap();

            // Mapeamento para PacienteContato (Entidade para ViewModel e vice-versa)
            CreateMap<PacienteContato, PacienteContatoViewModel>().ReverseMap();

            // Mapeamento para HistoricoPaciente (Entidade para ViewModel e vice-versa)
            CreateMap<HistoricoPaciente, HistoricoPacienteViewModel>().ReverseMap();

            // Mapeamento para FormacaoAcademicaProfissionalSaude (Entidade para ViewModel e vice-versa)
            CreateMap<FormacaoAcademicaProfissionalSaude, FormacaoAcademicaProfissionalSaudeViewModel>().ReverseMap();

            // Mapeamento para CursosCertificacoesProfissionalSaude (Entidade para ViewModel e vice-versa)
            CreateMap<CursosCertificacoesProfissionalSaude, CursosCertificacoesProfissionalSaudeViewModel>().ReverseMap();

            // Mapeamento para AgendamentoPaciente (Entidade para ViewModel e vice-versa)
            CreateMap<AgendamentoPaciente, AgendamentoPacienteViewModel>()
                .ForMember(dest => dest.Paciente, opt => opt.MapFrom(src => src.Paciente)); // Mapeia o objeto Paciente (entidade) para PacienteViewModel

            CreateMap<AgendamentoPacienteViewModel, AgendamentoPaciente>()
                .ForMember(dest => dest.Paciente, opt => opt.Ignore()); // Ignora o objeto de navegação ao mapear de ViewModel para Entidade
        }
    }
}