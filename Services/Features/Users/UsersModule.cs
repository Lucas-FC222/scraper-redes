using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Features.Users.Repositories;
using Shared.Infrastructure.Interfaces;

namespace Services.Features.Users
{
    /// <summary>
    /// Módulo responsável pela configuração de serviços e dependências relacionados a usuários.
    /// </summary>
    public class UsersModule : IModule
    {
        /// <summary>
        /// Configura os serviços e dependências do módulo de usuários no container de injeção de dependência.
        /// </summary>
        /// <param name="services">Coleção de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        public static void ConfigureModules(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
