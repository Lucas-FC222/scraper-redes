using Services.Features.Auth.Models;

namespace Services.Features.Users.Repositories
{
    /// <summary>
    /// Interface responsável pelo acesso e manipulação de dados de usuários na base de dados.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obtém um usuário pelo e-mail.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Usuário correspondente ao e-mail ou null se não encontrado.</returns>
        Task<AppUser?> GetByEmailAsync(string email);
        /// <summary>
        /// Obtém um usuário pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <returns>Usuário correspondente ao identificador ou null se não encontrado.</returns>
        Task<AppUser?> GetByIdAsync(Guid id);
        /// <summary>
        /// Obtém todos os usuários cadastrados.
        /// </summary>
        /// <returns>Coleção de usuários.</returns>
        Task<IEnumerable<AppUser>> GetAllAsync();
        /// <summary>
        /// Cria um novo usuário na base de dados.
        /// </summary>
        /// <param name="user">Usuário a ser criado.</param>
        /// <returns>Usuário criado.</returns>
        Task<AppUser?> CreateAsync(AppUser user);
        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="user">Usuário com dados atualizados.</param>
        Task UpdateAsync(AppUser user);
        /// <summary>
        /// Remove um usuário da base de dados pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        Task DeleteAsync(Guid id);
        /// <summary>
        /// Verifica se existe um usuário cadastrado com o e-mail informado.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>True se existir, caso contrário false.</returns>
        Task<bool> ExistsByEmailAsync(string email);
    }
}
