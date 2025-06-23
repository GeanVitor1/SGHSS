using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// Certifique-se de que o using para ApplicationUser está presente
using SGHSSVidaPlus.MVC.Data; // Inclui a definição de ApplicationUser

namespace SGHSSVidaPlus.MVC.Data
{
    // A classe do contexto deve herdar de IdentityDbContext<ApplicationUser>
    public class SGHSSVidaPlusMVCContext : IdentityDbContext<ApplicationUser> // <-- CORRIGIDO AQUI
    {
        public SGHSSVidaPlusMVCContext(DbContextOptions<SGHSSVidaPlusMVCContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}