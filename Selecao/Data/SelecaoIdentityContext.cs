using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Selecao.MVC.Data
{
    public class SelecaoIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public SelecaoIdentityContext(DbContextOptions<SelecaoIdentityContext> options):base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var hasher = new PasswordHasher<IdentityUser>();
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = "60832d0c-2593-4676-bae0-2009b94f62d1",
                UserName = "mjesus",
                NormalizedUserName = "MJESUS",
                Email = "mateus.jesus@daten.com.br",
                NormalizedEmail = "MATEUS.JESUS@DATEN.COM.BR",
                PasswordHash = hasher.HashPassword(null, "38205893")
            });

            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = "9752942c-3107-4646-b5dc-1faec0dd03ff",
                UserName = "gvitor",
                NormalizedUserName = "GVITOR",
                Email = "geandk36@gmail.com",
                NormalizedEmail = "GEANDK36@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "773636")
            });
        }
    }
}
