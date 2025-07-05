using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Linq;

namespace SGHSSVidaPlus.MVC.Additional
{
    public class CustomAuthorization
    {
        public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
        {
            return context.User.Identity.IsAuthenticated &&
                   context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
        }

        public static bool ValidarRoleUsuario(HttpContext context, string role)
        {
            return context.User.Identity.IsAuthenticated && context.User.IsInRole(role);
        }
    }

    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }
    }

    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "Identity", page = "/Account/Login", ReturnUrl = context.HttpContext.Request.Path.ToString() }));
                return;
            }

            // Se for admin, ele pode fazer tudo, independentemente das claims
            if (user.IsInRole("admin"))
                return;

            // NOVO: Se o usuário for um paciente, ele pode ter acesso a algumas funcionalidades específicas de paciente.
            // Aqui, você pode adicionar lógica específica para roles ou claims de paciente.
            // Por exemplo, se você quiser que pacientes acessem coisas específicas sem claims detalhadas:
            if (user.IsInRole("paciente") && _claim.Type == "paciente" && _claim.Value == "acesso_dashboard")
            {
                // Permite acesso ao dashboard do paciente
                return;
            }
            // Ou, se a claim específica for o suficiente:
            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
            {
                context.Result = new StatusCodeResult(403); // Acesso negado
            }
        }
    }
}