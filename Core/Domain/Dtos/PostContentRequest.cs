namespace Shared.Domain.Dtos
{
    /// <summary>
    /// Representa o conteúdo de um post nas redes sociais
    /// </summary>
    public class PostContentRequest
    {
        /// <summary>
        /// Identificador único do post
        /// </summary>
        public string Id { get; set; } = "";

        /// <summary>
        /// Texto ou conteúdo textual do post
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// URL da imagem associada ao post, se houver
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// URL do post original na rede social
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Tipo do post (ex: Facebook, Instagram)
        /// </summary>
        public string PostType { get; set; } = "";

        /// <summary>
        /// Data e hora de criação do post
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
