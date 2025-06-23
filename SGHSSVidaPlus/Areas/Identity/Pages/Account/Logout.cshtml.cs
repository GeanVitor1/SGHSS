// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SGHSSVidaPlus.MVC.Data; // <-- NAMESPACE ATUALIZADO para SGHSSVidaPlusIdentityContext e ApplicationUser

// NAMESPACE CORRIGIDO PARA O SEU PROJETO E ESTRUTURA DE ÁREAS
namespace SGHSSVidaPlus.MVC.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuário deslogado."); // Mensagem traduzida
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Isso precisa ser um redirecionamento para que o navegador faça uma nova
                // requisição e a identidade do usuário seja atualizada.
                return RedirectToPage();
            }
        }
    }
}