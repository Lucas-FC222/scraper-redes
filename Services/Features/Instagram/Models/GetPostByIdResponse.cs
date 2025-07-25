namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para a requisição de obtenção de um post do Instagram por identificador.
    /// </summary>
    public class GetPostByIdResponse
    {
        /// <summary>
        /// Post do Instagram retornado na resposta.
        /// </summary>
        public InstagramPost Post { get; set; } = new();
    }
}
