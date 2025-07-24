using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Configuration
{
    /// <summary>
    /// Configura opções do Swagger para API versionada
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// Construtor que recebe o provider de descrição de versão de API
        /// </summary>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Configura o Swagger para mostrar diferentes versões da API
        /// </summary>
        public void Configure(SwaggerGenOptions options)
        {
            // Adiciona um documento do Swagger para cada versão descoberta
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "API de Scraper de Redes Sociais",
                Version = description.ApiVersion.ToString(),
                Description = "API para coleta e análise de dados de redes sociais como Facebook e Instagram",
                Contact = new OpenApiContact()
                {
                    Name = "Equipe de Desenvolvimento",
                    Email = "dev@exemplo.com"
                },
                License = new OpenApiLicense()
                {
                    Name = "Uso Privado"
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " Esta versão da API está obsoleta e será descontinuada em breve.";
            }

            return info;
        }
    }
}
