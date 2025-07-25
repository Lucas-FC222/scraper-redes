using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Features.Instagram.Externals.Api.Client;
using Services.Features.Instagram.Repositories;
using Shared.Infrastructure.Interfaces;

namespace Services.Features.Instagram
{
    /// <summary>
    /// Módulo responsável pela configuração de serviços e dependências relacionados ao Instagram.
    /// </summary>
    public class InstagramModule : IModule
    {
        /// <summary>
        /// Configura os serviços e dependências do módulo Instagram no container de injeção de dependência.
        /// </summary>
        /// <param name="services">Coleção de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        public static void ConfigureModules(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IInstagramRepository, InstagramRepository>();
            services.AddScoped<IApifyInstagramClient, ApifyInstagramClient>();
        }
    }
}
