using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json.Serialization;
using Selecao.Configurations;
using Selecao.MVC.Additional;
using Selecao.MVC.Configurations;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigurarAutoMapper();
builder.Services.ConfigurarIdentity(builder.Configuration);
builder.Services.AddMvcConfiguration();

builder.Services.AddSignalR();
builder.Services.ResolverDependencias(builder.Configuration);
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => {options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;});
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();


var supportedCultures = new[] { new CultureInfo("pt-BR") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(localizationOptions);


app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
