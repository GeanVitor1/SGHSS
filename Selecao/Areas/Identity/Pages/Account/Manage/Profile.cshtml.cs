using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Selecao.MVC.Data;
using Selecao.MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Selecao.MVC.Areas.Identity.Pages.Account.Manage
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

            await _userManager.RemoveClaimAsync(user, new Claim("Nome", user.Nome));
            user.Nome = InputPerfil.Nome;
            user.Email = InputPerfil.Email;
            var result = await _userManager.UpdateAsync(user);

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
                    claim.IsSelected = (claimsUser.Any(c => c.Type == claim.Tipo && c.Value.Contains(claim.Valor)));

                claimsUsuario.RemoveAll(c => !c.IsSelected);
            }

            Usuario.Permissoes = claimsUsuario;
        }
    }
}
