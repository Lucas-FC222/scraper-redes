using Services.Features.Auth.Models;

namespace Services.Features.Users.Repositories
{
    /// <summary>
    /// Interface respons�vel pelo acesso e manipula��o de dados de usu�rios na base de dados.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obt�m um usu�rio pelo e-mail.
        /// </summary>
        /// <param name="email">E-mail do usu�rio.</param>
        /// <returns>Usu�rio correspondente ao e-mail ou null se n�o encontrado.</returns>
        Task<AppUser?> GetByEmailAsync(string email);
        /// <summary>
        /// Obt�m um usu�rio pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do usu�rio.</param>
        /// <returns>Usu�rio correspondente ao identificador ou null se n�o encontrado.</returns>
        Task<AppUser?> GetByIdAsync(Guid id);
        /// <summary>
        /// Obt�m todos os usu�rios cadastrados.
        /// </summary>
        /// <returns>Cole��o de usu�rios.</returns>
        Task<IEnumerable<AppUser>> GetAllAsync();
        /// <summary>
        /// Cria um novo usu�rio na base de dados.
        /// </summary>
        /// <param name="user">Usu�rio a ser criado.</param>
        /// <returns>Usu�rio criado.</returns>
        Task<AppUser?> CreateAsync(AppUser user);
        /// <summary>
        /// Atualiza os dados de um usu�rio existente.
        /// </summary>
        /// <param name="user">Usu�rio com dados atualizados.</param>
        Task UpdateAsync(AppUser user);
        /// <summary>
        /// Remove um usu�rio da base de dados pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do usu�rio.</param>
        Task DeleteAsync(Guid id);
        /// <summary>
        /// Verifica se existe um usu�rio cadastrado com o e-mail informado.
        /// </summary>
        /// <param name="email">E-mail do usu�rio.</param>
        /// <returns>True se existir, caso contr�rio false.</returns>
        Task<bool> ExistsByEmailAsync(string email);
    }
}
