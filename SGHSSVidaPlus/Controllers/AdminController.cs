using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGHSSVidaPlus.MVC.Data; // Namespace atualizado para o seu DbContext e ApplicationUser
using SGHSSVidaPlus.MVC.Models; // Namespace atualizado para seus ViewModels
using System.Security.Claims; // Necessário para a classe Claim
using System.Linq; // Necessário para métodos LINQ como ToList, Any, etc.
using System.Threading.Tasks; // Necessário para Task


namespace SGHSSVidaPlus.MVC.Controllers // Namespace atualizado
{
    [Authorize(Roles = "admin")] // Mantém a autorização para administradores
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
            return View(usuarios); // Retorna a lista de ApplicationUser (ou um ViewModel se preferir)
        }

        public IActionResult Incluir()
        {
            var usuario = new UsuarioViewModel
            {
                Permissoes = ClaimViewModel.ObterTodas() // Usa o método adaptado de ClaimViewModel
            };

            return View(usuario);
        }

        [HttpPost]
        public async Task<JsonResult> Incluir(UsuarioViewModel usuario)
        {
            var usuarioExiste = await _userManager.FindByNameAsync(usuario.Login);
            if (usuarioExiste != null)
                return Json(new { resultado = "falha", mensagem = "O usuário informado já existe" });

            // Removido !ModelState.IsValid aqui pois é uma validação comum e deve ser tratada no cliente ou com ModelState.AddModelError
            // e os erros seriam detalhados. Se for para Json, o string.Join abaixo é melhor.
            // if (!ModelState.IsValid)
            //    return Json(new { resultado = "falha", mensagem = "Ocorreu um erro desconhecido ao tentar incluir um novo usuário" });

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
            }

            var user = new ApplicationUser { UserName = usuario.Login, Email = usuario.Email, Nome = usuario.Nome, Admin = usuario.Admin }; // Adicionado 'Admin' e 'Nome'
            var result = await _userManager.CreateAsync(user, usuario.Password);
            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = string.Join(" ", result.Errors.Select(e => e.Description)) }); // Erros mais detalhados

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
                var claimsIncluir = new List<Claim>();

                foreach (var claimViewModel in claimsSelecionadas) // Itera sobre ClaimViewModel
                {
                    // As claims são adicionadas como Tipo e Valor, não agrupadas por tipo em uma string
                    claimsIncluir.Add(new Claim(claimViewModel.Tipo, claimViewModel.Valor));
                }

                claimsIncluir.Add(new Claim("Nome", usuario.Nome)); // Adiciona a claim de Nome

                result = await _userManager.AddClaimsAsync(user, claimsIncluir);
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário incluso, porém não foi possível incluir as permissões.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });
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

            // Mapeia ApplicationUser para UsuarioViewModel
            var usuario = new UsuarioViewModel()
            {
                Nome = usuarioBd.Nome,
                Login = usuarioBd.UserName, // UserName é o Login no Identity
                Email = usuarioBd.Email,
                Admin = usuarioBd.Admin // Carrega a propriedade Admin
            };

            var claimsUser = await _userManager.GetClaimsAsync(usuarioBd);
            var todasClaims = ClaimViewModel.ObterTodas();

            foreach (var claim in todasClaims)
            {
                // Verifica se a claim está presente nas claims do usuário
                claim.IsSelected = (claimsUser.Any(c => c.Type == claim.Tipo && c.Value == claim.Valor)); // Mudança para comparar o Valor exato
            }

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
                Bloqueado = usuarioBd.Bloqueado, // Carrega o status de Bloqueado
                Admin = usuarioBd.Admin // Carrega a propriedade Admin
            };

            var todasClaims = ClaimViewModel.ObterTodas();
            var role = await _userManager.GetRolesAsync(usuarioBd);
            if (role.Contains("admin"))
                usuario.Admin = true;
            else
            {
                var claimsUser = await _userManager.GetClaimsAsync(usuarioBd);
                foreach (var claim in todasClaims)
                    claim.IsSelected = (claimsUser.Any(c => c.Type == claim.Tipo && c.Value == claim.Valor)); // Mudança para comparar o Valor exato
            }

            usuario.Permissoes = todasClaims;

            return View(usuario);
        }

        [HttpPost]
        public async Task<JsonResult> Alterar(UsuarioViewModel usuario)
        {
            ModelState.Remove("Password"); // Remover validações de senha para alteração de perfil
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
            }

            var user = await _userManager.FindByNameAsync(usuario.Login);
            if (user == null)
                return Json(new { resultado = "falha", mensagem = "Não foi possível localizar os dados do usuário." });

            // Alterando usuario
            user.Nome = usuario.Nome;
            user.Email = usuario.Email;
            user.Bloqueado = usuario.Bloqueado;
            // A propriedade Admin agora está diretamente no ApplicationUser, então pode ser atualizada assim:
            user.Admin = usuario.Admin;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = "Não foi possível alterar usuário.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });

            // Alterando status de ADM (remoção e adição para garantir consistência)
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("admin") && !usuario.Admin) // Se era admin e não é mais
            {
                result = await _userManager.RemoveFromRoleAsync(user, "admin");
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário alterado, porém não foi possível remover o status de administrador.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });
            }
            else if (!currentRoles.Contains("admin") && usuario.Admin) // Se não era admin e agora é
            {
                var role = await _roleManager.FindByNameAsync("admin"); // Garante que a role "admin" exista
                if (role == null)
                {
                    role = new IdentityRole("admin"); // Cria a role se não existir
                    await _roleManager.CreateAsync(role);
                }
                result = await _userManager.AddToRoleAsync(user, "admin");
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário alterado, porém não foi possível incluir o status de administrador.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });
            }
            // Não precisa de um 'else' para quando o status de ADM não muda

            // Alterando Permissões (Claims)
            if (!usuario.Admin) // Apenas usuários não-admin têm permissões por claims
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                // Remove todas as claims de permissão que não são "Nome"
                var claimsPermissaoRemover = userClaims.Where(c => c.Type != "Nome").ToList(); // Assume que "Nome" é a única claim que deve persistir se não for admin
                result = await _userManager.RemoveClaimsAsync(user, claimsPermissaoRemover);
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário alterado, porém não foi possível remover permissões antigas.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });


                var claimsSelecionadas = usuario.Permissoes.Where(c => c.IsSelected);
                var claimsIncluir = new List<Claim>();

                foreach (var claimViewModel in claimsSelecionadas)
                {
                    claimsIncluir.Add(new Claim(claimViewModel.Tipo, claimViewModel.Valor));
                }

                // A claim "Nome" já deve existir e ser mantida ou adicionada se for o caso
                var nomeClaim = userClaims.FirstOrDefault(c => c.Type == "Nome");
                if (nomeClaim == null)
                    claimsIncluir.Add(new Claim("Nome", usuario.Nome)); // Adiciona a claim de Nome se não existir

                result = await _userManager.AddClaimsAsync(user, claimsIncluir);
                if (!result.Succeeded)
                    return Json(new { resultado = "falha", mensagem = "Usuário alterado, porém não foi possível incluir as novas permissões.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });

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

            user.Bloqueado = true; // Define a propriedade Bloqueado
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = "Não foi possível bloquear usuário.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });

            TempData["success"] = "Usuário Bloqueado com Sucesso!";

            await _signInManager.RefreshSignInAsync(user); // Garante que o estado de login seja atualizado

            return Json(new { resultado = "sucesso" });
        }

        [HttpPost]
        public async Task<JsonResult> DesbloquearUsuario(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { resultado = "falha", mensagem = "Não foi possível localizar os dados do usuário." });

            user.Bloqueado = false; // Define a propriedade Bloqueado
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Json(new { resultado = "falha", mensagem = "Não foi possível desbloquear usuário.Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });

            TempData["success"] = "Usuário Desbloqueado com Sucesso!";
            return Json(new { resultado = "sucesso" });
        }
    }
}