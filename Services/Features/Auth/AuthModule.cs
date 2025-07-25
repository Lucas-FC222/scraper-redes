using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Interfaces;

namespace Services.Features.Auth
{
    /// <summary>
    /// Módulo de autenticação responsável por registrar serviços relacionados à autenticação na injeção de dependência.
    /// </summary>
    public class AuthModule : IModule
    {
        /// <summary>
        /// Configura os serviços de autenticação no container de DI.
        /// </summary>
        /// <param name="services">Coleção de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        public static void ConfigureModules(IServiceCollection services, IConfiguration configuration)
        {
            
        }
    }
}
