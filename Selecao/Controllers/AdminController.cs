using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Selecao.MVC.Data;
using Selecao.MVC.Models;
using System.Data;
using System.Security.Claims;

namespace Selecao.MVC.Controllers
{

    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var usuarios = _userManager.Users.OrderBy(u => u.UserName).ToList();
            return View(usuarios);
        }

        public IActionResult Incluir()
        {
            var usuario = new UsuarioViewModel
            {
                Permissoes = ClaimViewModel.ObterTodas()
            };

            return View(usuario);
        }

        [HttpPost]
        public async Task<JsonResult> Incluir(UsuarioViewModel usuario)
        {
            var usuarioExiste = await _userManager.FindByNameAsync(usuario.Login);
            if (usuarioExiste != null)
                return Json(new { resultado = "falha", mensagem = "O usuário informado já existe" });

            if (!ModelState.IsValid)
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro desconhecido ao tentar incluir um novo usuário" });


            var user = new ApplicationUser { UserName = usuario.Login, Email = usuario.Email, Nome = usuario.Nome };
            var result = await _userManager.CreateAsync(user, usuario.Password);
            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = string.Join(".", result.Errors) });

            if (usuario.Admin)
            {
                var role = await _roleManager.FindByNameAsync("admin");
                if (role == null)
                {
                    role = new IdentityRole()
                    {
                        Name = "admin",
                        NormalizedName = "ADMIN"
                    };
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(user, role.Name);
            }
            else
            {
                var claimsSelecionadas = usuario.Permissoes.Where(c => c.IsSelected);
                var tipos = (from c in claimsSelecionadas select c.Tipo).Distinct();
                var claimsIncluir = new List<Claim>();

                foreach (var tipo in tipos)
                {
                    var valor = string.Join(",", (from c in claimsSelecionadas where c.Tipo == tipo select c.Valor));
                    claimsIncluir.Add(new Claim(tipo, valor));
                }

                claimsIncluir.Add(new Claim("Nome", usuario.Nome));

                result = await _userManager.AddClaimsAsync(user, claimsIncluir);
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário incluso, porém não foi possível incluir as permissões.Motivo: " + string.Join(".", result.Errors) });

            }

            TempData["success"] = "Usuário Incluído com Sucesso!";
            return Json(new { resultado = "sucesso" });
        }

        public async Task<IActionResult> Visualizar(string userId)
        {
            var usuarioBd = await _userManager.FindByIdAsync(userId);
            if (usuarioBd == null)
            {
                TempData["error"] = "Não foi possível localizar os dados do usuário.";
                return RedirectToAction("Index");
            }


            var usuario = new UsuarioViewModel()
            {
                Nome = usuarioBd.Nome,
                Login = usuarioBd.UserName,
                Email = usuarioBd.Email,
            };

            var claimsUser = await _userManager.GetClaimsAsync(usuarioBd);
            var todasClaims = ClaimViewModel.ObterTodas();

            foreach (var claim in todasClaims)
                claim.IsSelected = (claimsUser.Any(c => c.Type == claim.Tipo && c.Value.Contains(claim.Valor)));

            var role = await _userManager.GetRolesAsync(usuarioBd);
            if (role.Contains("admin"))
                usuario.Admin = true;

            usuario.Permissoes = todasClaims;

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Alterar(string userId)
        {
            var usuarioBd = await _userManager.FindByIdAsync(userId);
            if (usuarioBd == null)
            {
                TempData["error"] = "Não foi possível localizar os dados do usuário.";
                return RedirectToAction("Index");
            }


            var usuario = new UsuarioViewModel()
            {
                Nome = usuarioBd.Nome,
                Login = usuarioBd.UserName,
                Email = usuarioBd.Email,
            };

            var todasClaims = ClaimViewModel.ObterTodas();
            var role = await _userManager.GetRolesAsync(usuarioBd);
            if (role.Contains("admin"))
                usuario.Admin = true;
            else
            {
                var claimsUser = await _userManager.GetClaimsAsync(usuarioBd);
                foreach (var claim in todasClaims)
                    claim.IsSelected = (claimsUser.Any(c => c.Type == claim.Tipo && c.Value.Contains(claim.Valor)));
            }

            usuario.Permissoes = todasClaims;

            return View(usuario);
        }


        [HttpPost]
        public async Task<JsonResult> Alterar(UsuarioViewModel usuario)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro desconhecido ao tentar incluir um novo usuário" });

            var user = await _userManager.FindByNameAsync(usuario.Login);
            if (user == null)
                return Json(new { resultado = "falha", mensagem = "Não foi possível localizar os dados do usuário." });


            //Alterando usuario
            user.Nome = usuario.Nome;
            user.Email = usuario.Email;
            user.Bloqueado = usuario.Bloqueado;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = "Não foi possível alterar usuário.Motivo: " + string.Join(".", result.Errors) });

            //Alterando status de ADM
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("admin"))
            {
                result = await _userManager.RemoveFromRoleAsync(user, "admin");
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário alterado, porém não foi possível alterar as permissões.Motivo: " + string.Join(".", result.Errors) });

            }

            if (usuario.Admin)
            {
                result = await _userManager.AddToRoleAsync(user, "admin");
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário alterado, porém não foi possível incluir o status de administrador.Motivo: " + string.Join(".", result.Errors) });

            }
            else
            {
                //Alterando Permissões
                var userClaims = await _userManager.GetClaimsAsync(user);
                result = await _userManager.RemoveClaimsAsync(user, userClaims);
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário alterado, porém não foi possível alterar as permissões.Motivo: " + string.Join(".", result.Errors) });

                var claimsSelecionadas = usuario.Permissoes.Where(c => c.IsSelected);
                var tipos = (from c in claimsSelecionadas select c.Tipo).Distinct();
                var claimsIncluir = new List<Claim>();

                foreach (var tipo in tipos)
                {
                    var valor = string.Join(",", (from c in claimsSelecionadas where c.Tipo == tipo select c.Valor));
                    claimsIncluir.Add(new Claim(tipo, valor));
                }

                claimsIncluir.Add(new Claim("Nome", usuario.Nome));

                result = await _userManager.AddClaimsAsync(user, claimsIncluir);
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário incluso, porém não foi possível incluir as permissões.Motivo: " + string.Join(".", result.Errors) });

            }

            TempData["success"] = "Usuário Alterado com Sucesso!";
            return Json(new { resultado = "sucesso" });
        }

        [HttpPost]
        public async Task<JsonResult> BloquearUsuario(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { resultado = "falha", mensagem = "Não foi possível localizar os dados do usuário." });

            user.Bloqueado = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = "Não foi possível bloquear usuário.Motivo: " + string.Join(".", result.Errors) });

            TempData["success"] = "Usuário Bloqueado com Sucesso!";

            await _signInManager.RefreshSignInAsync(user);

            return Json(new { resultado = "sucesso" });
        }

        [HttpPost]
        public async Task<JsonResult> DesbloquearUsuario(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { resultado = "falha", mensagem = "Não foi possível localizar os dados do usuário." });

            user.Bloqueado = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = "Não foi possível bloquear usuário.Motivo: " + string.Join(".", result.Errors) });

            TempData["success"] = "Usuário Bloqueado com Sucesso!";
            return Json(new { resultado = "sucesso" });
        }
    }
}
