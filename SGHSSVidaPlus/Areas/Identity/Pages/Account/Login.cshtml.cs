using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SGHSSVidaPlus.MVC.Data;
using SGHSSVidaPlus.MVC.Models; // Adicionar para usar PacienteViewModel

namespace SGHSSVidaPlus.MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        // **NOVO:** Para injetar o PacienteService (precisamos do namespace SGHSSVidaPlus.Domain.Interfaces.Service)
        private readonly SGHSSVidaPlus.Domain.Interfaces.Service.IPacienteService _pacienteService;
        // **NOVO:** Para injetar o IMapper
        private readonly AutoMapper.IMapper _mapper;
        private readonly SGHSSVidaPlus.Domain.Interfaces.Repository.IPacienteRepository _pacienteRepository; // Para verificar CPF

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            SGHSSVidaPlus.Domain.Interfaces.Service.IPacienteService pacienteService, // Injeção
            AutoMapper.IMapper mapper, // Injeção
            SGHSSVidaPlus.Domain.Interfaces.Repository.IPacienteRepository pacienteRepository) // Injeção
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _pacienteService = pacienteService;
            _mapper = mapper;
            _pacienteRepository = pacienteRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // **NOVO:** Propriedade para a ViewModel de Registro de Paciente
        [BindProperty]
        public RegisterPacienteViewModel RegisterPatientInput { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O campo Usuário é obrigatório.")]
            [Display(Name = "Usuário")]
            public string Usuario { get; set; }

            [Required(ErrorMessage = "O campo Senha é obrigatório.")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [Display(Name = "Lembrar-me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            // Inicializa a RegisterPatientInput para evitar NullReferenceException
            RegisterPatientInput = new RegisterPacienteViewModel();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Usuario, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login inválido. Verifique suas credenciais.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        // NOVO: Action para o cadastro de paciente
        public async Task<IActionResult> OnPostRegisterPatientAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Garante que o Input original (login) não valide se estamos na aba de registro
            ModelState.Remove("Input.Usuario");
            ModelState.Remove("Input.Password");

            // Valida apenas a RegisterPatientInput
            if (ModelState.IsValid)
            {
                // 1. Verificar se o e-mail já existe no Identity
                var userExists = await _userManager.FindByEmailAsync(RegisterPatientInput.Email);
                if (userExists != null)
                {
                    ModelState.AddModelError(string.Empty, "O e-mail informado já está em uso.");
                    return Page();
                }

                // 2. Verificar se o CPF já existe no cadastro de pacientes (usando o PacienteRepository)
                var cpfExists = (await _pacienteRepository.BuscarPacientes(new Domain.ExtensionsParams.PacienteParams { CPF = RegisterPatientInput.CPF })).Any();
                if (cpfExists)
                {
                    ModelState.AddModelError(string.Empty, "O CPF informado já está cadastrado para outro paciente.");
                    return Page();
                }

                // 3. Criar o ApplicationUser para o paciente
                var user = new ApplicationUser
                {
                    UserName = RegisterPatientInput.Email, // Usar o e-mail como username para pacientes
                    Email = RegisterPatientInput.Email,
                    Nome = RegisterPatientInput.Nome,
                    Admin = false, // Pacientes não são administradores
                    EmailConfirmed = true, // Considere um processo de confirmação de e-mail real em produção
                    Bloqueado = false // Paciente não nasce bloqueado
                };

                var identityResult = await _userManager.CreateAsync(user, RegisterPatientInput.Senha);

                if (identityResult.Succeeded)
                {
                    // 4. Adicionar a role "paciente" ao usuário
                    var patientRoleExists = await _userManager.AddToRoleAsync(user, "paciente");
                    if (!patientRoleExists.Succeeded)
                    {
                        _logger.LogError($"Falha ao adicionar a role 'paciente' para o usuário {user.Email}: {string.Join(", ", patientRoleExists.Errors.Select(e => e.Description))}");
                        // Considere reverter a criação do usuário ou notificar o admin
                    }

                    // 5. Adicionar a Claim "Nome" para fácil acesso
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Nome", user.Nome));

                    // 6. Criar o Paciente no seu banco de dados de pacientes, vinculando ao UserId do Identity
                    var paciente = _mapper.Map<SGHSSVidaPlus.Domain.Entities.Paciente>(RegisterPatientInput);
                    paciente.UserId = user.Id; // Vincula o Paciente ao ApplicationUser.Id
                    paciente.UsuarioInclusao = user.Email; // Quem incluiu foi o próprio paciente se cadastrando
                    paciente.DataInclusao = DateTime.Now;
                    paciente.Ativo = true; // Paciente recém-cadastrado é ativo
                    paciente.Contatos.Add(new SGHSSVidaPlus.Domain.Entities.PacienteContato { Contato = RegisterPatientInput.Email, Tipo = "Email", IsWhatsApp = false });

                    // Opcional: Adicionar um contato inicial se ele preencheu algum campo de contato na ViewModel
                    // Se você tiver um campo de telefone na RegisterPacienteViewModel, adicione aqui.
                    // Exemplo: if (!string.IsNullOrEmpty(RegisterPatientInput.Telefone)) { paciente.Contatos.Add(new PacienteContato { Contato = RegisterPatientInput.Telefone, Tipo = "Telefone" }); }

                    // Se houver "TipoConsultaDesejada", crie um registro de agendamento inicial ou histórico
                    if (!string.IsNullOrEmpty(RegisterPatientInput.TipoConsultaDesejada))
                    {
                        paciente.Historico.Add(new SGHSSVidaPlus.Domain.Entities.HistoricoPaciente
                        {
                            Titulo = "Interesse de Consulta",
                            Descricao = $"Paciente manifestou interesse em consulta: {RegisterPatientInput.TipoConsultaDesejada}",
                            DataEvento = DateTime.Now,
                            ProfissionalResponsavel = "Sistema (Auto-cadastro)"
                        });

                        // Ou você pode criar um agendamento com status "Pendente" ou "Rascunho"
                        // Agendamento agendamentoInicial = new Agendamento
                        // {
                        //     Descricao = $"Consulta solicitada pelo paciente no auto-cadastro: {RegisterPatientInput.TipoConsultaDesejada}",
                        //     PacienteId = paciente.Id, // Note: paciente.Id só estará disponível após o SaveChanges
                        //     Status = "Pendente",
                        //     DataHoraAgendamento = DateTime.Now.AddDays(7), // Exemplo: agendar para daqui 7 dias, ou deixar em branco
                        //     // Outros campos de agendamento...
                        // };
                        // await _agendamentoService.Incluir(agendamentoInicial);
                    }

                    var pacienteResult = await _pacienteService.Incluir(paciente);

                    if (pacienteResult.Valido)
                    {
                        // 7. Login automático do paciente
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("Patient registered and logged in.");
                        TempData["success"] = "Cadastro realizado com sucesso! Você já está logado.";
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        // Se o cadastro do paciente falhar, tente remover o usuário do Identity
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError(string.Empty, $"Falha ao cadastrar paciente: {string.Join(" ", pacienteResult.Mensagens)}");
                        return Page();
                    }
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se o ModelState não for válido para RegisterPatientInput, ou se a criação do usuário/paciente falhar
            // Re-renderiza a página e mantém a aba de registro ativa.
            // Para isso, você pode adicionar um flag ao TempData ou ViewData.
            ViewData["ShowRegisterTab"] = true; // Use isso no Razor para manter a aba ativa
            return Page();
        }
    }
}