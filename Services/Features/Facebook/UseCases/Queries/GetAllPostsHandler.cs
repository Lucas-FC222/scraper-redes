using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Facebook.Models;
using Services.Features.Facebook.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Facebook.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de obtenção de todos os posts do Facebook,
    /// consultando o repositório e retornando o resultado padronizado.
    /// </summary>
    public class GetAllPostsHandler : IRequestHandler<GetAllPostsRequest, Result<GetAllPostsResponse>>
    {
        /// <summary>
        /// Repositório para consulta de posts do Facebook.
        /// </summary>
        private readonly IFacebookRepository _facebookRepository;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<GetAllPostsHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GetAllPostsHandler"/>.
        /// </summary>
        /// <param name="facebookRepository">Repositório de posts do Facebook.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public GetAllPostsHandler(IFacebookRepository facebookRepository, ILogger<GetAllPostsHandler> logger)
        {
            _facebookRepository = facebookRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para obter todos os posts do Facebook, retornando a lista ou erro caso não haja posts.
        /// </summary>
        /// <param name="request">Requisição para obtenção de todos os posts.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo a lista de posts ou detalhes do erro.</returns>
        public async Task<Result<GetAllPostsResponse>> Handle(GetAllPostsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando todos os posts do Facebook");

            var posts = await _facebookRepository.GetAllPostsAsync();

            if (!posts.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Nenhum post encontrado",
                    Detail = "Não foram encontrados posts no Facebook.",
                    Status = 404
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<GetAllPostsResponse>.Fail(problemDetails);
            }

            return Result<GetAllPostsResponse>.Ok(new GetAllPostsResponse
            {
                Posts = posts
            });
        }
    }
}
