using Shared.Infrastructure.Interfaces;
using System.Reflection;

namespace Api.Configuration
{
    /// <summary>
    /// Responsável por descobrir e configurar módulos que implementam <see cref="IModule"/> em assemblies fornecidos.
    /// </summary>
    public static class ModuleDiscovery
    {
        private static readonly Type ModuleType = typeof(IModule);

        /// <summary>
        /// Descobre e configura todos os módulos encontrados nos assemblies especificados.
        /// </summary>
        /// <param name="services">Coleção de serviços para DI.</param>
        /// <param name="assemblies">Assemblies onde buscar implementações de módulos.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        /// <exception cref="ArgumentException">Lançada se nenhum assembly for fornecido.</exception>
        public static void ConfigureModules(this IServiceCollection services, Assembly[] assemblies, IConfiguration configuration)
        {
            if (assemblies == null || !assemblies.Any())
            {
                throw new ArgumentException("No assemblies provided for module discovery.");
            }

            var moduleTypes = GetModuleTypes(assemblies);

            foreach (var type in moduleTypes)
            {
                var method = GetMapEndpointMethod(type);
                method?.Invoke(null, [services, configuration]);
            }
        }

        /// <summary>
        /// Obtém todos os tipos de módulos válidos nos assemblies fornecidos.
        /// </summary>
        /// <param name="assemblies">Assemblies para busca.</param>
        /// <returns>Tipos que implementam <see cref="IModule"/>.</returns>
        private static IEnumerable<Type> GetModuleTypes(params Assembly[] assemblies) =>
            assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => ModuleType.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });
        
        /// <summary>
        /// Obtém o método estático de configuração de módulos de um tipo.
        /// </summary>
        /// <param name="type">Tipo do módulo.</param>
        /// <returns>Informação do método <c>ConfigureModules</c> se existir; caso contrário, null.</returns>
        private static MethodInfo GetMapEndpointMethod(IReflect type) =>
            type.GetMethod(nameof(IModule.ConfigureModules), BindingFlags.Static | BindingFlags.Public)!;
    }
}
