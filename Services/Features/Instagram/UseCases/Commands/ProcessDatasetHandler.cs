using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Instagram.Externals.Api.Client;
using Services.Features.Instagram.Models;
using Services.Features.Instagram.Repositories;
using Shared.Domain.Models;

namespace Services.Features.Instagram.UseCases.Commands
{
    /// <summary>
    /// Handler responsável por processar a requisição de processamento de um dataset do Instagram,
    /// realizando classificação dos posts, persistência e salvamento de comentários, hashtags e menções.
    /// </summary>
    public class ProcessDatasetHandler : IRequestHandler<ProcessDatasetRequest, Result<ProcessDatasetResponse>>
    {
        /// <summary>
        /// Cliente para integração com a API Apify do Instagram.
        /// </summary>
        private readonly IApifyInstagramClient _apifyInstagramClient;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<ProcessDatasetHandler> _logger;
        /// <summary>
        /// Repositório para persistência e consulta de dados do Instagram.
        /// </summary>
        private readonly IInstagramRepository _instagramRepository;
        /// <summary>
        /// Instância do MediatR para envio de comandos e queries.
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ProcessDatasetHandler"/>.
        /// </summary>
        /// <param name="apifyInstagramClient">Cliente Apify para Instagram.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="instagramRepository">Repositório de dados do Instagram.</param>
        /// <param name="mediator">Instância do MediatR.</param>
        public ProcessDatasetHandler(IApifyInstagramClient apifyInstagramClient, ILogger<ProcessDatasetHandler> logger, IInstagramRepository instagramRepository, IMediator mediator)
        {
            _apifyInstagramClient = apifyInstagramClient;
            _logger = logger;
            _instagramRepository = instagramRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Processa a requisição para processar um dataset do Instagram, classificando os posts e salvando-os, além de comentários, hashtags e menções.
        /// </summary>
        /// <param name="request">Requisição contendo o identificador do dataset.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado do processamento, contendo os posts processados ou erro.</returns>
        public async Task<Result<ProcessDatasetResponse>> Handle(ProcessDatasetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processando dataset: {DatasetId}", request.DatasetId);

            var result = await _apifyInstagramClient.ProcessInstagramDatasetAsync(request.DatasetId);

            if (!result.Success)
            {
                return Result<ProcessDatasetResponse>.Fail(result.Error!);
            }

            if (result.Data == null || !result.Data.Posts.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Instagram scraper falhou",
                    Detail = $"Nenhum post encontrado no dataset do Instagram: {request.DatasetId}",
                    Status = 404
                };

                _logger.LogError(problemDetails.Detail, problemDetails.Status);

                return Result<ProcessDatasetResponse>.Fail(problemDetails);
            }

            _logger.LogInformation("Classificando {PostsCount} posts", result.Data.Posts.Count());

            foreach (var post in result.Data.Posts)
            {
                var ClassifyPostRequest = new Posts.Models.ClassifyPostRequest
                {
                    Text = post.Caption ?? ""
                };

                var classificationResult = await _mediator.Send(ClassifyPostRequest, cancellationToken);

                if (!classificationResult.Success)
                {
                    _logger.LogWarning("Classificação falhou para o post {PostId}: {Error}", post.Id, classificationResult.Error);
                    continue;
                }

                post.Topic = classificationResult?.Data?.Classification!;

                await _instagramRepository.SavePostsAsync(result.Data.Posts);
            }

            if (result.Data.Comments.Any())
            {
                await _instagramRepository.SaveCommentsAsync(result.Data.Comments);
            }

            if (result.Data.Hashtags.Any())
            {
                await _instagramRepository.SaveHashtagsAsync(result.Data.Hashtags);
            }

            if (result.Data.Mentions.Any())
            {
                await _instagramRepository.SaveMentionsAsync(result.Data.Mentions);
            }

            _logger.LogInformation("Todos os dados do dataset {DatasetId} foram processados e salvos.", request.DatasetId);
            
            return Result<ProcessDatasetResponse>.Ok(new ProcessDatasetResponse() { Posts = result.Data.Posts });
        }
    }
}
