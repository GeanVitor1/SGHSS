// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// REMOVER: using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared; // Não necessário
using SGHSSVidaPlus.MVC.Data; // <-- NAMESPACE ATUALIZADO para SGHSSVidaPlusIdentityContext e ApplicationUser
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Collections.Generic; // Adicionado para IList<AuthenticationScheme>
using System.Linq; // Adicionado para ToList()
using System.Threading.Tasks; // Adicionado para Task
using Microsoft.Extensions.Logging; // Adicionado para ILogger (se usar log)
// REMOVER: using static System.Runtime.InteropServices.JavaScript.JSType; // Não necessário

// NAMESPACE CORRIGIDO PARA O SEU PROJETO E ESTRUTURA DE ÁREAS
namespace SGHSSVidaPlus.MVC.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        /// directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        /// directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        /// directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        /// directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        /// directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            /// directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "O usuário não pode ser vazio")]
            public string Usuario { get; set; }

            /// <summary>
            /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            /// directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "A senha não pode ser vazia")] // Mensagem corrigida
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            /// directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Lembrar-me?")] // Traduzido
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
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByNameAsync(Input.Usuario);
                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuário não encontrado. Verifique seu nome de usuário e tente novamente.");
                    return Page();
                }

                if (usuario.Bloqueado) // Verifica a propriedade Bloqueado do ApplicationUser
                {
                    ModelState.AddModelError(string.Empty, "Usuário bloqueado!");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Usuario, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário logado.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Conta de usuário bloqueada.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login ou senha inválidos");
                    return Page();
                }
            }
            return Page();
        }
    }
}