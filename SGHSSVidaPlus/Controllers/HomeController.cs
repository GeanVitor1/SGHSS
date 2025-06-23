using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Necessário para [AllowAnonymous]
using Microsoft.Extensions.Logging;
using SGHSSVidaPlus.MVC.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace SGHSSVidaPlus.MVC.Controllers
{
    // REMOVER TEMPORARIAMENTE A ANOTAÇÃO [Authorize] DO CONTROLLER PRINCIPAL
    // [Authorize] 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // --- MÉTODO DE DEPURACAO TEMPORÁRIO (ACESSÍVEL SEM AUTENTICACAO) ---
        [HttpGet]
        [AllowAnonymous] // Permite acesso a este método sem login
        public IActionResult DebugUserClaims()
        {
            // Este método mostrará as claims e roles do usuário logado
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var isAuthenticated = User.Identity.IsAuthenticated;
            var authenticationType = User.Identity.AuthenticationType;
            var userName = User.Identity.Name;

            return Json(new
            {
                IsAuthenticated = isAuthenticated,
                AuthenticationType = authenticationType,
                UserName = userName,
                Roles = roles,
                Claims = claims
            });
        }
        // --- FIM DO MÉTODO DE DEPURACAO TEMPORÁRIO ---
    }
}