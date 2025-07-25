using Services.Features.Instagram.Models;

namespace Services.Features.Instagram.Repositories
{
    /// <summary>
    /// Interface respons�vel pelo acesso e manipula��o de dados de posts, coment�rios, hashtags e men��es do Instagram.
    /// </summary>
    public interface IInstagramRepository
    {
        /// <summary>
        /// Salva uma cole��o de posts do Instagram no banco de dados.
        /// </summary>
        /// <param name="posts">Cole��o de posts a serem salvos.</param>
        Task SavePostsAsync(IEnumerable<InstagramPost> posts);
        /// <summary>
        /// Salva uma cole��o de coment�rios do Instagram no banco de dados.
        /// </summary>
        /// <param name="comments">Cole��o de coment�rios a serem salvos.</param>
        Task SaveCommentsAsync(IEnumerable<InstagramComment> comments);
        /// <summary>
        /// Salva uma cole��o de hashtags do Instagram no banco de dados.
        /// </summary>
        /// <param name="hashtags">Cole��o de hashtags a serem salvas.</param>
        Task SaveHashtagsAsync(IEnumerable<InstagramHashtag> hashtags);
        /// <summary>
        /// Salva uma cole��o de men��es do Instagram no banco de dados.
        /// </summary>
        /// <param name="mentions">Cole��o de men��es a serem salvas.</param>
        Task SaveMentionsAsync(IEnumerable<InstagramMention> mentions);
        /// <summary>
        /// Obt�m todos os posts do Instagram cadastrados no banco de dados.
        /// </summary>
        /// <returns>Cole��o de posts do Instagram.</returns>
        Task<IEnumerable<InstagramPost>> GetAllPostsAsync();
        /// <summary>
        /// Obt�m um post do Instagram pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do post.</param>
        /// <returns>Post do Instagram correspondente ao ID informado ou null se n�o encontrado.</returns>
        Task<InstagramPost?> GetPostByIdAsync(string id);
        /// <summary>
        /// Obt�m uma cole��o de posts do Instagram pelos identificadores informados.
        /// </summary>
        /// <param name="ids">Lista de identificadores dos posts.</param>
        /// <returns>Cole��o de posts encontrados.</returns>
        Task<IEnumerable<InstagramPost>> GetPostsByIdsAsync(IEnumerable<string> ids);
        /// <summary>
        /// Obt�m todos os coment�rios de um post do Instagram pelo identificador do post.
        /// </summary>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>Cole��o de coment�rios do post.</returns>
        Task<IEnumerable<InstagramComment>> GetCommentsByPostIdAsync(string postId);
        /// <summary>
        /// Obt�m todas as hashtags de um post do Instagram pelo identificador do post.
        /// </summary>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>Cole��o de hashtags do post.</returns>
        Task<IEnumerable<InstagramHashtag>> GetHashTagsByPostIdAsync(string postId);
        /// <summary>
        /// Obt�m todas as men��es de um post do Instagram pelo identificador do post.
        /// </summary>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>Cole��o de men��es do post.</returns>
        Task<IEnumerable<InstagramMention>> GetMentionsByPostIdAsync(string postId);
        /// <summary>
        /// Pesquisa posts do Instagram por uma lista de palavras-chave.
        /// </summary>
        /// <param name="keywords">Palavras-chave para pesquisa nos posts.</param>
        /// <returns>Cole��o de posts que correspondem �s palavras-chave.</returns>
        Task<IEnumerable<InstagramPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
}