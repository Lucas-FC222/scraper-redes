using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Posts.Models
{
    /// <summary>
    /// Requisição para classificar o texto de um post.
    /// </summary>
    public class ClassifyPostRequest : IRequest<Result<ClassifyPostReponse>>
    {
        /// <summary>
        /// Texto do post a ser classificado.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
