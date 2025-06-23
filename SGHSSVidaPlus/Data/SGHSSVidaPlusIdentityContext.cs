using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

            // Geração de Hash para a senha (necessário para o Identity)
            var hasher = new PasswordHasher<ApplicationUser>(); // Usando ApplicationUser

            // Dados do usuário ADMINISTRADOR padrão
            var adminUser = new ApplicationUser
            {
                Id = "b743329b-2839-4d64-968b-f417b7b9f847", // UM NOVO GUID ÚNICO PARA O ID DO USUÁRIO
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@sghssvidaplus.com.br",
                NormalizedEmail = "ADMIN@SGHSSVIDAPLUS.COM.BR",
                EmailConfirmed = true, // Confirma o e-mail para que possa logar sem confirmação
                PasswordHash = hasher.HashPassword(null, "Admin@123"), // Senha: Admin@123 (MUDE ISSO EM PRODUÇÃO!)
                Nome = "Administrador Master", // Propriedade Nome do ApplicationUser
                Admin = true, // Define como Admin = true
                Bloqueado = false, // Não bloqueado
                SecurityStamp = Guid.NewGuid().ToString() // Necessário para o Identity
            };

            // Dados da Role ADMINISTRADOR
            var adminRole = new IdentityRole
            {
                Id = "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2", // UM NOVO GUID ÚNICO PARA O ID DA ROLE
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Adiciona o usuário ao modelo
            builder.Entity<ApplicationUser>().HasData(adminUser);

            // Adiciona a role ao modelo
            builder.Entity<IdentityRole>().HasData(adminRole);

            // Associa o usuário admin à role admin
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRole.Id,
                    UserId = adminUser.Id
                }
            );

            // NOTA: Se você tinha dados de seed antigos, esta nova migração os sobrescreverá/adicionará.
            // Para testar, use o login: admin / Senha: Admin@123
        }
    }
}