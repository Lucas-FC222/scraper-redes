using Services.Features.Facebook.Models;

namespace Services.Features.Facebook.Repositories
{
    /// <summary>
    /// Interface responsável pelo acesso e manipulação de dados de posts do Facebook na base de dados.
    /// </summary>
    public interface IFacebookRepository
    {
        /// <summary>
        /// Salva uma coleção de posts do Facebook no banco de dados.
        /// </summary>
        /// <param name="posts">Coleção de posts a serem salvos.</param>
        Task SavePostsAsync(IEnumerable<FacebookPost> posts);

        /// <summary>
        /// Obtém todos os posts do Facebook cadastrados no banco de dados.
        /// </summary>
        /// <returns>Coleção de posts do Facebook.</returns>
        Task<IEnumerable<FacebookPost>> GetAllPostsAsync();

        /// <summary>
        /// Obtém um post do Facebook pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do post.</param>
        /// <returns>Post do Facebook correspondente ao ID informado ou null se não encontrado.</returns>
        Task<FacebookPost?> GetPostByIdAsync(string id);

        /// <summary>
        /// Pesquisa posts do Facebook por uma lista de palavras-chave.
        /// </summary>
        /// <param name="keywords">Palavras-chave para pesquisa nos posts.</param>
        /// <returns>Coleção de posts que correspondem às palavras-chave.</returns>
        Task<IEnumerable<FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
}