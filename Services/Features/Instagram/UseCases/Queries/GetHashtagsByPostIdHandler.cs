using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Instagram.Models;
using Services.Features.Instagram.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Instagram.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de obtenção de hashtags de um post do Instagram,
    /// consultando o repositório e retornando o resultado padronizado.
    /// </summary>
    public class GetHashtagsByPostIdHandler : IRequestHandler<GetHashTagsByPostIdRequest, Result<GetHashTagsByPostIdResponse>>
    {
        /// <summary>
        /// Repositório para consulta de hashtags do Instagram.
        /// </summary>
        private readonly IInstagramRepository _instagramRepository;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<GetHashtagsByPostIdHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GetHashtagsByPostIdHandler"/>.
        /// </summary>
        /// <param name="instagramRepository">Repositório de hashtags do Instagram.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public GetHashtagsByPostIdHandler(IInstagramRepository instagramRepository, ILogger<GetHashtagsByPostIdHandler> logger)
        {
            _instagramRepository = instagramRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para obter hashtags de um post do Instagram, retornando a lista ou erro caso não haja hashtags.
        /// </summary>
        /// <param name="request">Requisição para obtenção de hashtags.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo a lista de hashtags ou detalhes do erro.</returns>
        public async Task<Result<GetHashTagsByPostIdResponse>> Handle(GetHashTagsByPostIdRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando todas as hashtags do Instagram");

            var HashTags = await _instagramRepository.GetHashTagsByPostIdAsync(request.PostId);

            if (!HashTags.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Nenhuma hashtag encontrada",
                    Detail = "Não foram encontrados hashtags no Instagram.",
                    Status = 404
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<GetHashTagsByPostIdResponse>.Fail(problemDetails);
            }

            return Result<GetHashTagsByPostIdResponse>.Ok(new GetHashTagsByPostIdResponse
            {
                HashTags = HashTags
            });
        }
    }
}
