using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface de módulo para configuração de serviços e dependências em módulos da aplicação.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Configura os serviços e dependências do módulo no container de injeção de dependência.
        /// </summary>
        /// <param name="services">Coleção de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        static abstract void ConfigureModules(IServiceCollection services, IConfiguration configuration);
    }
}
