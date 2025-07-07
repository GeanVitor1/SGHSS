using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity; // Garanta que este using está presente
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using SGHSSVidaPlus.MVC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.MVC.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IPacienteService _pacienteService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IMapper _mapper;

        public LoginModel(SignInManager<ApplicationUser> signInManager, // <--- LINHA CORRIGIDA AQUI
                                      UserManager<ApplicationUser> userManager,
                                      ILogger<LoginModel> logger,
                                      IPacienteService pacienteService,
                                      IAgendamentoService agendamentoService,
                                      IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _pacienteService = pacienteService;
            _agendamentoService = agendamentoService;
            _mapper = mapper;
        }

        public InputModel Input { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public RegisterPatientInputModel RegisterInput { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O campo Usuário é obrigatório.")]
            [Display(Name = "Usuário")]
            public string Usuario { get; set; }

            [Required(ErrorMessage = "O campo Senha é obrigatório.")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }
        }

        public class RegisterPatientInputModel
        {
            [Required(ErrorMessage = "É necessário informar o nome completo.")]
            [Display(Name = "Nome Completo")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "É necessário informar a data de nascimento.")]
            [DataType(DataType.Date)]
            [Display(Name = "Data de Nascimento")]
            public DateTime DataNascimento { get; set; }

            [Required(ErrorMessage = "É necessário informar o CPF.")]
            [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 ou 14 caracteres.")]
            [Display(Name = "CPF")]
            public string CPF { get; set; }

            [Required(ErrorMessage = "É necessário informar o endereço.")]
            [Display(Name = "Endereço Completo")]
            public string Endereco { get; set; }

            [Required(ErrorMessage = "É necessário informar o estado civil.")]
            [Display(Name = "Estado Civil")]
            public string EstadoCivil { get; set; }

            [Required(ErrorMessage = "É necessário informar o e-mail.")]
            [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Required(ErrorMessage = "É necessário informar a senha.")]
            [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Senha { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Senha")]
            [Compare("Senha", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
            public string ConfirmarSenha { get; set; }

            [Display(Name = "Tipo de Consulta Desejada")]
            public string? TipoConsultaDesejada { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;

            if (TempData.ContainsKey("success"))
            {
                ViewData["ShowSuccessAlert"] = TempData["success"].ToString();
            }
            else if (TempData.ContainsKey("error"))
            {
                // Adiciona o erro do TempData ao ModelState para ser exibido no ValidationSummary ou span asp-validation-for
                ModelState.AddModelError(string.Empty, TempData["error"].ToString());
                if (TempData.ContainsKey("FormIdWithErrors"))
                {
                    ViewData["FormIdWithErrors"] = TempData["FormIdWithErrors"].ToString();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(InputModel input, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            Input = input;

            if (!ModelState.IsValid)
            {
                // Coleta apenas erros gerais (não associados a campos específicos do input que podem ser tratados por asp-validation-for)
                var generalErrors = ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Where(e => string.IsNullOrEmpty(e.ErrorMessage) || ModelState.Keys.All(k => k != e.ErrorMessage)) // Tentativa de filtrar erros duplicados
                                                .Select(e => e.ErrorMessage)
                                                .Distinct() // Garante que não haja mensagens repetidas aqui
                                                .ToList();
                TempData["error"] = string.Join(" . ", generalErrors);
                ViewData["FormIdWithErrors"] = "account";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Input.Usuario, Input.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(Input.Usuario);
                if (user != null && user.Bloqueado)
                {
                    await _signInManager.SignOutAsync();
                    _logger.LogWarning("Usuário '{UserName}' tentou logar mas está bloqueado.", Input.Usuario);
                    TempData["error"] = "Sua conta está bloqueada. Entre em contato com o administrador.";
                    ViewData["FormIdWithErrors"] = "account";
                    return Page();
                }

                _logger.LogInformation("Usuário logado.");
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Conta de usuário bloqueada.");
                TempData["error"] = "Sua conta está temporariamente bloqueada devido a várias tentativas de login falhas. Por favor, tente novamente mais tarde.";
                ViewData["FormIdWithErrors"] = "account";
                return Page();
            }
            else
            {
                TempData["error"] = "Tentativa de login inválida. Verifique seu usuário e senha.";
                ViewData["FormIdWithErrors"] = "account";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRegisterPatientAsync(RegisterPatientInputModel registerInput, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            RegisterInput = registerInput;

            // Antes de tentar criar o usuário, verifique a validação do modelo RegisterInput
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .Distinct() // Adicionado .Distinct() para remover mensagens duplicadas de validação de campo
                                       .ToList();
                TempData["error"] = string.Join(" . ", errors);
                ViewData["FormIdWithErrors"] = "registerForm";
                return Page();
            }

            var userExists = await _userManager.FindByEmailAsync(RegisterInput.Email);
            if (userExists != null)
            {
                TempData["error"] = "Este e-mail já está em uso. Por favor, faça login ou use outro e-mail.";
                ViewData["FormIdWithErrors"] = "registerForm";
                return Page();
            }

            var pacienteExists = (await _pacienteService.BuscarPacientes(new Domain.ExtensionsParams.PacienteParams { CPF = RegisterInput.CPF })).FirstOrDefault();
            if (pacienteExists != null)
            {
                TempData["error"] = "Este CPF já está cadastrado em nosso sistema. Por favor, faça login ou entre em contato.";
                ViewData["FormIdWithErrors"] = "registerForm";
                return Page();
            }

            var user = _mapper.Map<ApplicationUser>(RegisterInput);
            user.UserName = RegisterInput.Email;
            user.EmailConfirmed = true;
            user.Admin = false;
            user.Bloqueado = false;

            var identityResult = await _userManager.CreateAsync(user, RegisterInput.Senha);

            if (identityResult.Succeeded)
            {
                _logger.LogInformation("Novo usuário paciente criado com sucesso.");

                if (!await _userManager.IsInRoleAsync(user, "paciente"))
                {
                    await _userManager.AddToRoleAsync(user, "paciente");
                }
                await _userManager.AddClaimAsync(user, new Claim("TipoUsuario", "Paciente"));

                var paciente = _mapper.Map<Paciente>(RegisterInput);
                paciente.ApplicationUserId = user.Id;
                paciente.Ativo = true;
                paciente.UsuarioInclusao = user.UserName;
                paciente.DataInclusao = DateTime.Now;

                var pacienteResult = await _pacienteService.Incluir(paciente);

                if (pacienteResult.Valido)
                {
                    if (!string.IsNullOrWhiteSpace(RegisterInput.TipoConsultaDesejada))
                    {
                        var agendamento = new Agendamento
                        {
                            PacienteId = paciente.Id,
                            Descricao = RegisterInput.TipoConsultaDesejada,
                            // DataHoraAgendamento pode ser uma data futura genérica,
                            // pois o agendamento real será ajustado pelo admin
                            DataHoraAgendamento = DateTime.Now.AddDays(7), // Ex: daqui a uma semana
                            Status = "Pendente", // <-- Status inicial "Pendente"
                            Encerrado = false,
                            UsuarioInclusao = user.UserName,
                            DataInclusao = DateTime.Now,
                            ProfissionalResponsavelId = 1 // <-- ID do atendente padrão
                        };
                        var agendamentoResult = await _agendamentoService.Incluir(agendamento);
                        if (!agendamentoResult.Valido)
                        {
                            _logger.LogError("Falha ao criar agendamento inicial para o paciente {PatientId}: {Errors}", paciente.Id, string.Join(", ", agendamentoResult.Mensagens));
                            // Opcional: Adicionar uma mensagem de erro ao TempData se o agendamento inicial falhar,
                            // mesmo que o cadastro do paciente tenha sucesso.
                            // TempData["error"] = "Seu cadastro foi realizado, mas houve um problema ao criar seu agendamento inicial. Por favor, entre em contato.";
                        }
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("Usuário paciente '{Email}' cadastrado e logado.", RegisterInput.Email);
                    TempData["success"] = "Cadastro realizado com sucesso! Você já está logado.";
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    await _userManager.DeleteAsync(user); // Desfaz a criação do usuário se o paciente não puder ser criado
                    TempData["error"] = string.Join(" . ", pacienteResult.Mensagens);
                    ViewData["FormIdWithErrors"] = "registerForm";
                    return Page();
                }
            }
            else // Caso o identityResult.Succeeded seja false (erros do Identity)
            {
                // Coleta apenas as descrições dos erros do Identity, que já virão traduzidas (se a localização estiver configurada)
                var identityErrors = identityResult.Errors.Select(e => e.Description).Distinct().ToList();

                // Adiciona esses erros ao ModelState.
                // O Tag Helper asp-validation-for já deverá pegar e exibir nos campos corretos.
                // Para erros gerais (como regras de senha), eles serão adicionados ao ModelState com string.Empty como chave.
                foreach (var error in identityErrors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                // Passa as mensagens de erro do Identity para o TempData, para serem exibidas pelo SweetAlert
                TempData["error"] = string.Join(" . ", identityErrors);
                ViewData["FormIdWithErrors"] = "registerForm";
                return Page();
            }
        }
    }
}