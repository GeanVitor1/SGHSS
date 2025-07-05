using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System; // Adicionado para Guid

namespace SGHSSVidaPlus.MVC.Data
{
    public class SGHSSVidaPlusIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public SGHSSVidaPlusIdentityContext(DbContextOptions<SGHSSVidaPlusIdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var hasher = new PasswordHasher<ApplicationUser>();

            // Dados do usuário ADMINISTRADOR padrão
            var adminUser = new ApplicationUser
            {
                Id = "b743329b-2839-4d64-968b-f417b7b9f847",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@sghssvidaplus.com.br",
                NormalizedEmail = "ADMIN@SGHSSVIDAPLUS.COM.BR",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Admin@123"),
                Nome = "Administrador Master",
                Admin = true,
                Bloqueado = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Dados da Role ADMINISTRADOR
            var adminRole = new IdentityRole
            {
                Id = "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Dados da Nova Role PACIENTE
            var patientRole = new IdentityRole
            {
                Id = "c8f2a1b3-d4e5-4f6a-g7h8-i9j0k1l2m3n4", // NOVO GUID para a role paciente
                Name = "paciente",
                NormalizedName = "PACIENTE",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Adiciona o usuário Admin e as Roles ao modelo
            builder.Entity<ApplicationUser>().HasData(adminUser);
            builder.Entity<IdentityRole>().HasData(adminRole, patientRole); // Adiciona as duas roles

            // Associa o usuário admin à role admin
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRole.Id,
                    UserId = adminUser.Id
                }
            );

            // NÃO associe o usuário admin à role paciente aqui, a menos que seja um cenário específico.
            // O paciente será associado à role 'paciente' quando ele se cadastrar.
        }
    }
}