using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration
using Microsoft.Extensions.DependencyInjection; // Necessário para IServiceCollection
using SGHSSVidaPlus.MVC.Data; // Namespace do seu DbContext de Identity atualizado
using System; // Necessário para TimeSpan

namespace SGHSSVidaPlus.MVC.Configurations // Namespace atualizado
{
    public static class IdentityConfig
    {
        public static IServiceCollection ConfigurarIdentity(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddIdentity<ApplicationUser, IdentityRole>(options => // <-- USANDO ApplicationUser
            {
                // Configurações da política de senha
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddDefaultUI() // Permite usar as Razor Pages de Identity padrão (Login, Register, etc.)
                .AddEntityFrameworkStores<SGHSSVidaPlusMVCContext>() // <-- USANDO SEU NOVO DBCONTEXT DE IDENTITY
                .AddDefaultTokenProviders(); // Para tokens de recuperação de senha, etc.

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
                options.LoginPath = new PathString("/Identity/Account/Login");

            });

            return services;
        }
    }
}