using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Instagram.Models;
using Services.Features.Instagram.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Instagram.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de obtenção de um post do Instagram por identificador,
    /// consultando o repositório e retornando o resultado padronizado.
    /// </summary>
    public class GetPostByIdHandler : IRequestHandler<GetPostByIdRequest, Result<GetPostByIdResponse>>
    {
        /// <summary>
        /// Repositório para consulta de posts do Instagram.
        /// </summary>
        private readonly IInstagramRepository _InstagramRepository;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<GetPostByIdHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GetPostByIdHandler"/>.
        /// </summary>
        /// <param name="InstagramRepository">Repositório de posts do Instagram.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public GetPostByIdHandler(IInstagramRepository InstagramRepository, ILogger<GetPostByIdHandler> logger)
        {
            _InstagramRepository = InstagramRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para obter um post do Instagram pelo identificador, retornando o post ou erro caso não encontrado.
        /// </summary>
        /// <param name="request">Requisição contendo o identificador do post.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo o post ou detalhes do erro.</returns>
        public async Task<Result<GetPostByIdResponse>> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando post do Instagram com ID: {Id}", request.Id);

            var post = await _InstagramRepository.GetPostByIdAsync(request.Id);

            if (post is null)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Post não encontrado",
                    Detail = $"Não foram encontrados posts no Instagram com o Id {request.Id}.",
                    Status = 404
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<GetPostByIdResponse>.Fail(problemDetails);
            }

            return Result<GetPostByIdResponse>.Ok(new GetPostByIdResponse
            {
                Post = post
            });
        }
    }
}
