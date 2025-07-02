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
        public async Task<JsonResult> Incluir([FromBody] UsuarioViewModel usuario)
        {
            // 1. Validação de existência do usuário por Login
            var usuarioExiste = await _userManager.FindByNameAsync(usuario.Login);
            if (usuarioExiste != null)
            {
                return Json(new { resultado = "falha", mensagem = "O login informado já está em uso." });
            }

            // 2. Validação do ModelState (validações via Data Annotations na UsuarioViewModel)
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();
                // Se o erro for de duplicidade de e-mail (gerenciado pelo Identity), adicione uma mensagem mais amigável
                if (erros.Any(e => e.Contains("Email", StringComparison.OrdinalIgnoreCase) && e.Contains("is already taken", StringComparison.OrdinalIgnoreCase)))
                {
                    return Json(new { resultado = "falha", mensagem = "O e-mail informado já está em uso." });
                }
                return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
            }

            // 3. Criação do ApplicationUser
            var user = new ApplicationUser
            {
                UserName = usuario.Login,
                Email = usuario.Email,
                Nome = usuario.Nome,
                Admin = usuario.Admin, // Propriedade personalizada para indicar se é Admin
                EmailConfirmed = true // Considere confirmar o e-mail automaticamente ou por um processo de ativação
            };

            // Tenta criar o usuário com a senha
            var result = await _userManager.CreateAsync(user, usuario.Password);

            // 4. Tratamento de falha na criação do usuário pelo Identity
            if (!result.Succeeded)
            {
                // Identity Errors são geralmente muito descritivos (ex: "Passwords must have at least one digit.")
                // Mapeia os erros do Identity para mensagens legíveis.
                var identityErrors = result.Errors.Select(e =>
                {
                    if (e.Code == "DuplicateUserName") return "O login já existe.";
                    if (e.Code == "DuplicateEmail") return "O e-mail já existe.";
                    if (e.Code == "PasswordTooShort") return $"A senha é muito curta. Mínimo {e.Description.Split(' ').LastOrDefault()} caracteres.";
                    if (e.Code == "PasswordRequiresNonAlphanumeric") return "A senha deve conter pelo menos um caractere não alfanumérico.";
                    if (e.Code == "PasswordRequiresDigit") return "A senha deve conter pelo menos um número ('0'-'9').";
                    if (e.Code == "PasswordRequiresLower") return "A senha deve conter pelo menos uma letra minúscula ('a'-'z').";
                    if (e.Code == "PasswordRequiresUpper") return "A senha deve conter pelo menos uma letra maiúscula ('A'-'Z').";
                    return e.Description; // Retorna a descrição padrão para outros erros
                }).ToList();

                return Json(new { resultado = "falha", mensagem = string.Join(" ", identityErrors) });
            }

            // 5. Atribuição de Role (Administrador)
            if (usuario.Admin)
            {
                var role = await _roleManager.FindByNameAsync("admin");
                if (role == null)
                {
                    // Cria a role "admin" se ela não existir. Isso deve ser feito no Seed ou em migrações.
                    // Mas para garantir que não pare o fluxo, podemos criar aqui também.
                    role = new IdentityRole()
                    {
                        Name = "admin",
                        NormalizedName = "ADMIN"
                    };
                    await _roleManager.CreateAsync(role);
                }
                // Adiciona o usuário à role "admin"
                result = await _userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    // Se falhar, é um erro crítico, pois o usuário foi criado mas a role não foi atribuída.
                    // Poderíamos deletar o usuário ou logar um erro para correção manual.
                    return Json(new { resultado = "falha", mensagem = "Usuário incluído, porém não foi possível atribuir o status de administrador. Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });
                }
            }
            // 6. Atribuição de Permissões (Claims) para usuários não-admin
            else // Se o usuário NÃO for administrador, atribua as claims de permissão
            {
                var claimsSelecionadas = usuario.Permissoes.Where(c => c.IsSelected).ToList();
                var claimsIncluir = new List<Claim>();

                foreach (var claimViewModel in claimsSelecionadas)
                {
                    // Adiciona a Claim com Tipo e Valor
                    claimsIncluir.Add(new Claim(claimViewModel.Tipo, claimViewModel.Valor));
                }

                // Adiciona a Claim "Nome" que é importante para ser acessada via User.FindFirstValue("Nome")
                claimsIncluir.Add(new Claim("Nome", user.Nome));

                result = await _userManager.AddClaimsAsync(user, claimsIncluir);
                if (!result.Succeeded)
                {
                    return Json(new { resultado = "falha", mensagem = "Usuário incluído, porém não foi possível incluir as permissões. Motivo: " + string.Join(" ", result.Errors.Select(e => e.Description)) });
                }
            }

            // 7. Sucesso!
            // Usa TempData para passar a mensagem de sucesso que será exibida no _Layout.cshtml
            TempData["success"] = "Usuário Incluído com Sucesso!";
            // Retorna sucesso com URL de redirecionamento
            return Json(new { resultado = "sucesso", redirectUrl = Url.Action("Index", "Admin") });
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