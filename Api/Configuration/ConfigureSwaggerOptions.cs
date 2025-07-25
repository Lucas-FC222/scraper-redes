using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Configuration
{
    /// <summary>
    /// Configura opções do Swagger para suportar versionamento de API.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ConfigureSwaggerOptions"/>.
        /// </summary>
        /// <param name="provider">Provider de descrições de versão da API.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Configura o Swagger para exibir diferentes versões da API, adicionando um documento para cada versão descoberta.
        /// </summary>
        /// <param name="options">Opções de configuração do Swagger.</param>
        public void Configure(SwaggerGenOptions options)
        {
            // Adiciona um documento do Swagger para cada versão descoberta
            if (_provider.ApiVersionDescriptions.Any())
            {
                foreach (var description in _provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        description.GroupName,
                        CreateInfoForApiVersion(description));
                }
            }
            else
            {
                // Fallback para garantir que pelo menos um documento seja criado
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API de Scraper de Redes Sociais",
                    Version = "1.0",
                    Description = "API para coleta e análise de dados de redes sociais como Facebook e Instagram"
                });
            }
        }

        /// <summary>
        /// Cria as informações de exibição do Swagger para uma versão específica da API.
        /// </summary>
        /// <param name="description">Descrição da versão da API.</param>
        /// <returns>Objeto <see cref="OpenApiInfo"/> configurado para a versão.</returns>
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
