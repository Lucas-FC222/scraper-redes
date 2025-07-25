using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Features.Posts.Externals.Client;
using Shared.Infrastructure.Interfaces;

namespace Services.Features.Posts
{
    /// <summary>
    /// Módulo responsável pela configuração de serviços e dependências relacionados a posts.
    /// </summary>
    public class PostsModule : IModule
    {
        /// <summary>
        /// Configura os serviços e dependências do módulo de posts no container de injeção de dependência.
        /// </summary>
        /// <param name="services">Coleção de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        public static void ConfigureModules(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGroqApiClient, GroqApiClient>();
        }
    }
}
