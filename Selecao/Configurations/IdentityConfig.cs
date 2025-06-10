using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Selecao.MVC.Data;

namespace Selecao.MVC.Configurations
{
    public static class IdentityConfig
    {
        public static IServiceCollection ConfigurarIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SelecaoIdentityContext>(options => options.UseSqlServer(configuration.GetConnectionString("Selecao")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Configurações da política de senha
                options.Password.RequireDigit = false;          // Exige ao menos um dígito (0-9)
                options.Password.RequireLowercase = false;      // Exige ao menos uma letra minúscula (a-z)
                options.Password.RequireUppercase = false;      // Exige ao menos uma letra maiúscula (A-Z)
                options.Password.RequireNonAlphanumeric = false; // Exige ao menos um caractere especial (!, @, #, etc.)
                options.Password.RequiredLength = 8;           // Comprimento mínimo da senha
                options.Password.RequiredUniqueChars = 1;      // Número mínimo de caracteres únicos na senha
            })
                .AddDefaultUI()
                .AddEntityFrameworkStores<SelecaoIdentityContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
                options.LoginPath = new PathString("/Login");
            });

            return services;
        }
    }
}
