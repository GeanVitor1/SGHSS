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

// Contexto principal do Hospital, só pra dados da app mesmo
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Contexto do Identity
builder.Services.AddDbContext<SGHSSVidaPlusMVCContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly("SGHSSVidaPlus.Infrastructure.Data") // Migrations vão pra Infrastructure.Data
    )
);

// Configura Identity com ApplicationUser e roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role; // Importante para roles funcionarem direito
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<SGHSSVidaPlusMVCContext>() // Liga Identity com seu contexto
.AddDefaultTokenProviders();

// Adicionar a política de autorização para pacientes
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequirePacienteRoleOrClaim", policy =>
        policy.RequireRole("paciente").RequireClaim("TipoUsuario", "Paciente"));
    // Você pode ter uma policy que exija apenas a role, ou apenas a claim, ou ambas.
    // Use 'RequireRole("paciente")' se você for adicionar a role "paciente" a todos os pacientes.
    // Use 'RequireClaim("TipoUsuario", "Paciente")' se preferir usar apenas a claim.
    // A combinação é a mais segura se você usar os dois.
});


builder.Services.AddAutoMapper(typeof(SGHSSVidaPlus.MVC.Configurations.AutoMapperConfig));

builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IProfissionalSaudeRepository, ProfissionalSaudeRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IProfissionalSaudeService, ProfissionalSaudeService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

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

// Seed de dados inicial
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var pacienteService = scope.ServiceProvider.GetRequiredService<IPacienteService>(); // Pegar o PacienteService

    // Seed para o usuário ADMINISTRADOR
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null) // Se o admin não existir, cria
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@sghssvidaplus.com.br",
            NormalizedEmail = "ADMIN@SGHSSVIDAPLUS.COM.BR",
            EmailConfirmed = true,
            Nome = "Administrador Master",
            Admin = true,
            Bloqueado = false,
        };
        var createResult = await userManager.CreateAsync(adminUser, "Admin@123");
        if (createResult.Succeeded)
        {
            if (!await roleManager.RoleExistsAsync("admin"))
                await roleManager.CreateAsync(new IdentityRole("admin"));
            await userManager.AddToRoleAsync(adminUser, "admin");
            await userManager.AddClaimAsync(adminUser, new Claim("paciente", "visualizar"));
            // Adicione outras claims para admin se necessário
        }
    }
    else // Se o admin já existe, garanta que ele tem a role e a claim
    {
        if (!await userManager.IsInRoleAsync(adminUser, "admin"))
        {
            if (!await roleManager.RoleExistsAsync("admin"))
                await roleManager.CreateAsync(new IdentityRole("admin"));
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
        var claims = await userManager.GetClaimsAsync(adminUser);
        if (!claims.Any(c => c.Type == "paciente" && c.Value == "visualizar"))
        {
            await userManager.AddClaimAsync(adminUser, new Claim("paciente", "visualizar"));
        }
    }

    // NOVO: Seed para a role "paciente" (se não existir)
    if (!await roleManager.RoleExistsAsync("paciente"))
    {
        await roleManager.CreateAsync(new IdentityRole("paciente"));
    }

    // OPCIONAL: Criar um profissional padrão se não houver nenhum
    // Para que o agendamento inicial no cadastro do paciente não quebre
    // caso ProfissionalResponsavelId seja obrigatório e não haja profissionais cadastrados.
    // Você precisaria de um IProfissionalSaudeService aqui também.
    // var profissionalService = scope.ServiceProvider.GetRequiredService<IProfissionalSaudeService>();
    // var profissionalPadrao = (await profissionalService.BuscarProfissional(new ProfissionalSaudeParams { Nome = "Profissional Padrão" })).FirstOrDefault();
    // if (profissionalPadrao == null)
    // {
    //     await profissionalService.Incluir(new ProfissionalSaude { Nome = "Profissional Padrão", Cargo = "Geral", Ativo = true, /* Outros campos obrigatórios */ });
    // }
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