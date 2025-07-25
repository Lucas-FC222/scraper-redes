using Services.Features.Instagram.Models;

namespace Services.Features.Instagram.Repositories
{
    /// <summary>
    /// Interface responsável pelo acesso e manipulação de dados de posts, comentários, hashtags e menções do Instagram.
    /// </summary>
    public interface IInstagramRepository
    {
        /// <summary>
        /// Salva uma coleção de posts do Instagram no banco de dados.
        /// </summary>
        /// <param name="posts">Coleção de posts a serem salvos.</param>
        Task SavePostsAsync(IEnumerable<InstagramPost> posts);
        /// <summary>
        /// Salva uma coleção de comentários do Instagram no banco de dados.
        /// </summary>
        /// <param name="comments">Coleção de comentários a serem salvos.</param>
        Task SaveCommentsAsync(IEnumerable<InstagramComment> comments);
        /// <summary>
        /// Salva uma coleção de hashtags do Instagram no banco de dados.
        /// </summary>
        /// <param name="hashtags">Coleção de hashtags a serem salvas.</param>
        Task SaveHashtagsAsync(IEnumerable<InstagramHashtag> hashtags);
        /// <summary>
        /// Salva uma coleção de menções do Instagram no banco de dados.
        /// </summary>
        /// <param name="mentions">Coleção de menções a serem salvas.</param>
        Task SaveMentionsAsync(IEnumerable<InstagramMention> mentions);
        /// <summary>
        /// Obtém todos os posts do Instagram cadastrados no banco de dados.
        /// </summary>
        /// <returns>Coleção de posts do Instagram.</returns>
        Task<IEnumerable<InstagramPost>> GetAllPostsAsync();
        /// <summary>
        /// Obtém um post do Instagram pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do post.</param>
        /// <returns>Post do Instagram correspondente ao ID informado ou null se não encontrado.</returns>
        Task<InstagramPost?> GetPostByIdAsync(string id);
        /// <summary>
        /// Obtém uma coleção de posts do Instagram pelos identificadores informados.
        /// </summary>
        /// <param name="ids">Lista de identificadores dos posts.</param>
        /// <returns>Coleção de posts encontrados.</returns>
        Task<IEnumerable<InstagramPost>> GetPostsByIdsAsync(IEnumerable<string> ids);
        /// <summary>
        /// Obtém todos os comentários de um post do Instagram pelo identificador do post.
        /// </summary>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>Coleção de comentários do post.</returns>
        Task<IEnumerable<InstagramComment>> GetCommentsByPostIdAsync(string postId);
        /// <summary>
        /// Obtém todas as hashtags de um post do Instagram pelo identificador do post.
        /// </summary>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>Coleção de hashtags do post.</returns>
        Task<IEnumerable<InstagramHashtag>> GetHashTagsByPostIdAsync(string postId);
        /// <summary>
        /// Obtém todas as menções de um post do Instagram pelo identificador do post.
        /// </summary>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>Coleção de menções do post.</returns>
        Task<IEnumerable<InstagramMention>> GetMentionsByPostIdAsync(string postId);
        /// <summary>
        /// Pesquisa posts do Instagram por uma lista de palavras-chave.
        /// </summary>
        /// <param name="keywords">Palavras-chave para pesquisa nos posts.</param>
        /// <returns>Coleção de posts que correspondem às palavras-chave.</returns>
        Task<IEnumerable<InstagramPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
}