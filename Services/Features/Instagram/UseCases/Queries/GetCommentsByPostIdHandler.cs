using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Instagram.Models;
using Services.Features.Instagram.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Instagram.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de obtenção de comentários de um post do Instagram,
    /// consultando o repositório e retornando o resultado padronizado.
    /// </summary>
    public class GetCommentsByPostIdHandler : IRequestHandler<GetCommentsByPostIdRequest, Result<GetCommentsByPostIdResponse>>
    {
        /// <summary>
        /// Repositório para consulta de comentários do Instagram.
        /// </summary>
        private readonly IInstagramRepository _InstagramRepository;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<GetCommentsByPostIdHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GetCommentsByPostIdHandler"/>.
        /// </summary>
        /// <param name="InstagramRepository">Repositório de comentários do Instagram.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public GetCommentsByPostIdHandler(IInstagramRepository InstagramRepository, ILogger<GetCommentsByPostIdHandler> logger)
        {
            _InstagramRepository = InstagramRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para obter comentários de um post do Instagram, retornando a lista ou erro caso não haja comentários.
        /// </summary>
        /// <param name="request">Requisição para obtenção de comentários.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo a lista de comentários ou detalhes do erro.</returns>
        public async Task<Result<GetCommentsByPostIdResponse>> Handle(GetCommentsByPostIdRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando todos os comentários do Instagram");

            var comments = await _InstagramRepository.GetCommentsByPostIdAsync(request.PostId);

            if (!comments.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Nenhum post encontrado",
                    Detail = "Não foram encontrados comentários no Instagram.",
                    Status = 404
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<GetCommentsByPostIdResponse>.Fail(problemDetails);
            }

            return Result<GetCommentsByPostIdResponse>.Ok(new GetCommentsByPostIdResponse
            {
                Comments = comments
            });
        }
    }
}
