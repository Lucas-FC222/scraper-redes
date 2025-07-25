namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Resposta para a requisição de obtenção de um post do Facebook por identificador.
    /// </summary>
    public class GetPostByIdResponse
    {
        /// <summary>
        /// Post do Facebook retornado na resposta.
        /// </summary>
        public FacebookPost Post { get; set; } = new();
    }
}
