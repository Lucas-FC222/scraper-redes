using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Instagram.Models;
using Services.Features.Instagram.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Instagram.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de obtenção de menções de um post do Instagram,
    /// consultando o repositório e retornando o resultado padronizado.
    /// </summary>
    public class GetMentionsByPostIdHandler : IRequestHandler<GetMentionsByPostIdRequest, Result<GetMentionsByPostIdResponse>>
    {
        /// <summary>
        /// Repositório para consulta de menções do Instagram.
        /// </summary>
        private readonly IInstagramRepository _InstagramRepository;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<GetMentionsByPostIdHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GetMentionsByPostIdHandler"/>.
        /// </summary>
        /// <param name="InstagramRepository">Repositório de menções do Instagram.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public GetMentionsByPostIdHandler(IInstagramRepository InstagramRepository, ILogger<GetMentionsByPostIdHandler> logger)
        {
            _InstagramRepository = InstagramRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para obter menções de um post do Instagram, retornando a lista ou erro caso não haja menções.
        /// </summary>
        /// <param name="request">Requisição para obtenção de menções.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo a lista de menções ou detalhes do erro.</returns>
        public async Task<Result<GetMentionsByPostIdResponse>> Handle(GetMentionsByPostIdRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando todos as menções do Instagram");

            var Mentions = await _InstagramRepository.GetMentionsByPostIdAsync(request.PostId);

            if (!Mentions.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Nenhuma menção encontrada",
                    Detail = "Não foram encontrados menções no Instagram.",
                    Status = 404
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<GetMentionsByPostIdResponse>.Fail(problemDetails);
            }

            return Result<GetMentionsByPostIdResponse>.Ok(new GetMentionsByPostIdResponse
            {
                Mentions = Mentions
            });
        }
    }
}
