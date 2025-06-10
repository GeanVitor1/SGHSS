using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Text.Json.Serialization;

namespace Selecao.Configurations
{
    public static class MvcConfig
    {
        public static IServiceCollection AddMvcConfiguration(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

            //Para windows com IIS ultiliza essa configuração
            services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = 500 * 1024 * 1024);

            //Aceita upload de vídeo de até 5GB
            services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 500 * 1024 * 1024; });

            // Adiciona serviços de controladores ao container de injeção de dependência e configura opções de serialização JSON.
            // Isso é útil quando você tem um modelo que inclui referências circulares (por exemplo, um objeto que se refere a outro, que por sua vez se refere ao primeiro).
            // Quando ReferenceHandler.Preserve está ativado, o serializador JSON preserva referências circulares usando um formato específico, garantindo que o processo de serialização não cause exceções e que as referências sejam mantidas no objeto JSON resultante.
            services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; });
            services.AddRazorPages();
            services.AddMvc(options =>
            {
                options.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider());
            }).AddSessionStateTempDataProvider();
            services.AddSession();
            services.Configure<FormOptions>(options =>
            {
                //Indica o tamanho maximo de chave e valor que a aplicação suporta
                options.ValueCountLimit = 5000;
                //Indica o tamanho em MB do body
                options.MultipartBodyLengthLimit = 512 * 1024 * 1024;
            });

            return services;
        }
        // Mantem campos do tipo String Vazio, sem essa configuração o ASP.NET Core transforma o vazio em null automaticamente
        public class CustomMetadataProvider : IMetadataDetailsProvider, IDisplayMetadataProvider
        {
            public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
            {
                if (context.Key.MetadataKind == ModelMetadataKind.Property)
                    //Altere para true ou delete a função para transformar strings vazias em null
                    context.DisplayMetadata.ConvertEmptyStringToNull = false;
            }
        }
    }
}
