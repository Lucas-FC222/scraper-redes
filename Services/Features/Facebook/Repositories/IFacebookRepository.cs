using Services.Features.Facebook.Models;

namespace Services.Features.Facebook.Repositories
{
    /// <summary>
    /// Interface respons�vel pelo acesso e manipula��o de dados de posts do Facebook na base de dados.
    /// </summary>
    public interface IFacebookRepository
    {
        /// <summary>
        /// Salva uma cole��o de posts do Facebook no banco de dados.
        /// </summary>
        /// <param name="posts">Cole��o de posts a serem salvos.</param>
        Task SavePostsAsync(IEnumerable<FacebookPost> posts);

        /// <summary>
        /// Obt�m todos os posts do Facebook cadastrados no banco de dados.
        /// </summary>
        /// <returns>Cole��o de posts do Facebook.</returns>
        Task<IEnumerable<FacebookPost>> GetAllPostsAsync();

        /// <summary>
        /// Obt�m um post do Facebook pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do post.</param>
        /// <returns>Post do Facebook correspondente ao ID informado ou null se n�o encontrado.</returns>
        Task<FacebookPost?> GetPostByIdAsync(string id);

        /// <summary>
        /// Pesquisa posts do Facebook por uma lista de palavras-chave.
        /// </summary>
        /// <param name="keywords">Palavras-chave para pesquisa nos posts.</param>
        /// <returns>Cole��o de posts que correspondem �s palavras-chave.</returns>
        Task<IEnumerable<FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
}