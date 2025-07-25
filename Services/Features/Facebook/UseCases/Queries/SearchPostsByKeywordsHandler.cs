using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Facebook.Models;
using Services.Features.Facebook.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Facebook.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de pesquisa de posts do Facebook por palavras-chave,
    /// consultando o repositório e retornando o resultado padronizado.
    /// </summary>
    public class SearchPostsByKeywordsHandler : IRequestHandler<SearchPostsByKeywordsRequest, Result<SearchPostsByKeywordsResponse>>
    {
        /// <summary>
        /// Repositório para consulta de posts do Facebook.
        /// </summary>
        private readonly IFacebookRepository _facebookRepository;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<SearchPostsByKeywordsHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="SearchPostsByKeywordsHandler"/>.
        /// </summary>
        /// <param name="facebookRepository">Repositório de posts do Facebook.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public SearchPostsByKeywordsHandler(IFacebookRepository facebookRepository, ILogger<SearchPostsByKeywordsHandler> logger)
        {
            _facebookRepository = facebookRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para pesquisar posts do Facebook por palavras-chave, retornando a lista ou erro caso não haja posts.
        /// </summary>
        /// <param name="request">Requisição contendo as palavras-chave para pesquisa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo a lista de posts ou detalhes do erro.</returns>
        public async Task<Result<SearchPostsByKeywordsResponse>> Handle(SearchPostsByKeywordsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando posts do Facebook por palavra chave");

            var posts = await _facebookRepository.SearchPostsByKeywordsAsync(request.Keywords);

            if (!posts.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Nenhum post encontrado",
                    Detail = "Não foram encontrados posts no Facebook.",
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
