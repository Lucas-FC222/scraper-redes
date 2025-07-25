using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Features.Facebook.Externals.Api.Client;
using Services.Features.Facebook.Repositories;
using Shared.Infrastructure.Interfaces;

namespace Services.Features.Facebook
{
    /// <summary>
    /// Módulo responsável pela configuração de serviços e dependências relacionados ao Facebook.
    /// </summary>
    public class FacebookModule : IModule
    {
        /// <summary>
        /// Configura os serviços e dependências do módulo Facebook no container de injeção de dependência.
        /// </summary>
        /// <param name="services">Coleção de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        public static void ConfigureModules(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFacebookRepository, FacebookRepository>();
            services.AddScoped<IApifyFacebookClient, ApifyFacebookClient>();
        }
    }
}
