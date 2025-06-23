// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGHSSVidaPlus.MVC.Data;
using SGHSSVidaPlus.MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims; // Para System.Security.Claims.Claim
using System.Linq; // Para métodos LINQ
using System.Collections.Generic; // Para List
using System.Threading.Tasks; // Para Task

namespace SGHSSVidaPlus.MVC.Areas.Identity.Pages.Account.Manage
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModelPerfil InputPerfil { get; set; }

        [BindProperty]
        public InputModelSenha InputSenha { get; set; }

        public UsuarioViewModel Usuario { get; set; }

        public class InputModelPerfil
        {
            [Required]
            [MinLength(3, ErrorMessage = "O nome deve ter pelo menos 3 caracteres")]
            public string Nome { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
        }

        public class InputModelSenha
        {

            [DataType(DataType.Password)]
            public string OldPassword { get; set; }

            public string NewPassword { get; set; }

            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadPageDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.Nome = InputPerfil.Nome;
            user.Email = InputPerfil.Email;
            var result = await _userManager.UpdateAsync(user);

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var nomeClaim = existingClaims.FirstOrDefault(c => c.Type == "Nome");
            if (nomeClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, nomeClaim);
            }
            await _userManager.AddClaimAsync(user, new Claim("Nome", user.Nome));

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await LoadPageDataAsync();
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(InputSenha.OldPassword))
            {
                if (!string.IsNullOrWhiteSpace(InputSenha.NewPassword))
                {
                    if (InputSenha.NewPassword != InputSenha.ConfirmPassword)
                    {
                        ModelState.AddModelError(string.Empty, "A confirmação e a nova senha não conferem");
                        await LoadPageDataAsync();
                        return Page();
                    }
                    else
                    {
                        var changePasswordResult = await _userManager.ChangePasswordAsync(user, InputSenha.OldPassword, InputSenha.NewPassword);
                        if (!changePasswordResult.Succeeded)
                        {
                            foreach (var error in changePasswordResult.Errors)
                                ModelState.AddModelError(string.Empty, error.Description);

                            await LoadPageDataAsync();
                            return Page();
                        }
                    }
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["success"] = "Perfil Alterado Com Sucesso";
            return RedirectToAction("Index", "Home");
        }

        private async Task LoadPageDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Usuario = new UsuarioViewModel()
            {
                Login = user.UserName,
                Nome = user.Nome,
                Email = user.Email
            };

            var claimsUser = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            Usuario.Admin = roles.Contains("admin");

            var claimsUsuario = ClaimViewModel.ObterTodas();

            if (Usuario.Admin)
            {
                claimsUsuario = claimsUsuario.Select(c => new ClaimViewModel(c.Tipo, c.Valor, true)).ToList();
            }
            else
            {
                foreach (var claim in claimsUsuario)
                    // CORREÇÃO AQUI: c.Value (do System.Security.Claims.Claim)
                    claim.IsSelected = (claimsUser.Any(c => c.Type == claim.Tipo && c.Value == claim.Valor));

                claimsUsuario = claimsUsuario.Where(c => c.IsSelected).ToList();
            }

            Usuario.Permissoes = claimsUsuario;
        }
    }
}