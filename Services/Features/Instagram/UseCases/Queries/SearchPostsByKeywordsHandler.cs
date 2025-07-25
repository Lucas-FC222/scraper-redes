using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Instagram.Models;
using Services.Features.Instagram.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Instagram.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de pesquisa de posts do Instagram por palavras-chave,
    /// consultando o repositório e retornando o resultado padronizado.
    /// </summary>
    public class SearchPostsByKeywordsHandler : IRequestHandler<SearchPostsByKeywordsRequest, Result<SearchPostsByKeywordsResponse>>
    {
        /// <summary>
        /// Repositório para consulta de posts do Instagram.
        /// </summary>
        private readonly IInstagramRepository _InstagramRepository;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<SearchPostsByKeywordsHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="SearchPostsByKeywordsHandler"/>.
        /// </summary>
        /// <param name="InstagramRepository">Repositório de posts do Instagram.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public SearchPostsByKeywordsHandler(IInstagramRepository InstagramRepository, ILogger<SearchPostsByKeywordsHandler> logger)
        {
            _InstagramRepository = InstagramRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para pesquisar posts do Instagram por palavras-chave, retornando a lista ou erro caso não haja posts.
        /// </summary>
        /// <param name="request">Requisição contendo as palavras-chave para pesquisa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo a lista de posts ou detalhes do erro.</returns>
        public async Task<Result<SearchPostsByKeywordsResponse>> Handle(SearchPostsByKeywordsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando posts do Instagram por palavra chave");

            var posts = await _InstagramRepository.SearchPostsByKeywordsAsync(request.Keywords);

            if (!posts.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Nenhum post encontrado",
                    Detail = "Não foram encontrados posts no Instagram.",
                    Status = 404
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<SearchPostsByKeywordsResponse>.Fail(problemDetails);
            }

            return Result<SearchPostsByKeywordsResponse>.Ok(new SearchPostsByKeywordsResponse
            {
                Posts = posts
            });
        }
    }
}
