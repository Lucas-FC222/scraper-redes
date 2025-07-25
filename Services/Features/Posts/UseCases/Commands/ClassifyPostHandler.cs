using MediatR;
using Services.Features.Posts.Externals.Client;
using Services.Features.Posts.Models;
using Shared.Domain.Models;

namespace Services.Features.Posts.UseCases.Commands
{
    /// <summary>
    /// Handler responsável por processar a requisição de classificação de um post,
    /// utilizando a API Groq para identificar o tema do texto.
    /// </summary>
    public class ClassifyPostHandler : IRequestHandler<ClassifyPostRequest, Result<ClassifyPostReponse>>
    {
        /// <summary>
        /// Cliente para integração com a API Groq.
        /// </summary>
        private readonly IGroqApiClient _groqApiClient;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ClassifyPostHandler"/>.
        /// </summary>
        /// <param name="groqApiClient">Cliente para classificação de posts via API Groq.</param>
        public ClassifyPostHandler(IGroqApiClient groqApiClient)
        {
            _groqApiClient = groqApiClient;
        }

        /// <summary>
        /// Processa a requisição para classificar o texto de um post, retornando o tema identificado.
        /// </summary>
        /// <param name="request">Requisição contendo o texto do post.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da classificação, contendo o tema identificado.</returns>
        public async Task<Result<ClassifyPostReponse>> Handle(ClassifyPostRequest request, CancellationToken cancellationToken)
        {
           var result = await _groqApiClient.ClassifyPostAsync(request.Text);
            return Result<ClassifyPostReponse>.Ok(new ClassifyPostReponse() { Classification = result?.Data! });
        }
    }
}
