using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json.Serialization;
using System.Globalization;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SGHSSVidaPlus.MVC.Data;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Infrastructure.Data.Context;
using SGHSSVidaPlus.Infrastructure.Data.Repositories;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using SGHSSVidaPlus.Application.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Security.Claims; // Necessário para ClaimTypes
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddDbContext<SGHSSVidaPlusMVCContext>(options =>
    options.UseSqlServer(connectionString));

// CORREÇÃO AQUI: Adicionado options.ClaimsIdentity.RoleClaimType
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // Garante que o RoleManager adicione claims de tipo ClaimTypes.Role ao principal do usuário
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role; // <-- ADIÇÃO CRÍTICA AQUI
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<SGHSSVidaPlusMVCContext>()
.AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(SGHSSVidaPlus.MVC.Configurations.AutoMapperConfig));

builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IProfissionalSaudeRepository, ProfissionalSaudeRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<ITipoAtendimentoRepository, TipoAtendimentoRepository>();

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IProfissionalSaudeService, ProfissionalSaudeService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<ITipoAtendimentoService, TipoAtendimentoService>();

builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

builder.Services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = 500 * 1024 * 1024);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500 * 1024 * 1024;
    options.ValueCountLimit = 5000;
});

builder.Services.AddControllersWithViews(options =>
{
    options.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider());
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

var supportedCultures = new[] { new CultureInfo("pt-BR") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();

app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser != null)
    {
        var claims = await userManager.GetClaimsAsync(adminUser);
        if (!claims.Any(c => c.Type == "paciente" && c.Value == "visualizar"))
        {
            await userManager.AddClaimAsync(adminUser, new Claim("paciente", "visualizar"));
        }

        if (!await userManager.IsInRoleAsync(adminUser, "admin"))
        {
            if (!await roleManager.RoleExistsAsync("admin"))
                await roleManager.CreateAsync(new IdentityRole("admin"));

            await userManager.AddToRoleAsync(adminUser, "admin");
        }
    }
}

app.Run();

public class CustomMetadataProvider : IMetadataDetailsProvider, IDisplayMetadataProvider
{
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (context.Key.MetadataKind == ModelMetadataKind.Property)
            context.DisplayMetadata.ConvertEmptyStringToNull = false;
    }
}