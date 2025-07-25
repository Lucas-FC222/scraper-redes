using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Facebook.Externals.Api.Client;
using Services.Features.Facebook.Models;
using Services.Features.Facebook.Repositories;
using Shared.Domain.Models;
using System.Data;

namespace Services.Features.Facebook.UseCases.Commands
{
    /// <summary>
    /// Handler responsável por processar a requisição de processamento de um dataset do Facebook,
    /// realizando classificação dos posts e persistência no banco de dados.
    /// </summary>
    public class ProcessDatasetHandler : IRequestHandler<ProcessDatasetRequest, Result<ProcessDatasetResponse>>
    {
        /// <summary>
        /// Cliente para integração com a API Apify do Facebook.
        /// </summary>
        private readonly IApifyFacebookClient _apifyFacebookClient;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<ProcessDatasetHandler> _logger;
        /// <summary>
        /// Repositório para persistência e consulta de posts do Facebook.
        /// </summary>
        private readonly IFacebookRepository _facebookRepository;
        /// <summary>
        /// Instância do MediatR para envio de comandos e queries.
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ProcessDatasetHandler"/>.
        /// </summary>
        /// <param name="apifyFacebookClient">Cliente Apify para Facebook.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="facebookRepository">Repositório de posts do Facebook.</param>
        /// <param name="mediator">Instância do MediatR.</param>
        public ProcessDatasetHandler(IApifyFacebookClient apifyFacebookClient, ILogger<ProcessDatasetHandler> logger, IFacebookRepository facebookRepository, IMediator mediator)
        {
            _apifyFacebookClient = apifyFacebookClient;
            _logger = logger;
            _facebookRepository = facebookRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Processa a requisição para processar um dataset do Facebook, classificando os posts e salvando-os.
        /// </summary>
        /// <param name="request">Requisição contendo o identificador do dataset.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado do processamento, contendo os posts processados ou erro.</returns>
        public async Task<Result<ProcessDatasetResponse>> Handle(ProcessDatasetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processando dataset do Facebook: {DatasetId}", request.DatasetId);

            var result = await _apifyFacebookClient.ProcessFacebookDatasetAsync(request.DatasetId);

            if (!result.Success)
            {
                return Result<ProcessDatasetResponse>.Fail(result.Error!);
            }

            if (result.Data == null || !result.Data.Posts.Any())
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Facebook scraper falhou",
                    Detail = $"Nenhum post encontrado no dataset do Facebook: {request.DatasetId}",
                    Status = 404
                };

                _logger.LogError(problemDetails.Detail, problemDetails.Status);

                return Result<ProcessDatasetResponse>.Fail(problemDetails);
            }

            // Filtrar duplicatas de Id
            var postsUnicos = result.Data.Posts
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var post in postsUnicos)
            {
                var ClassifyPostRequest = new Posts.Models.ClassifyPostRequest
                {
                    Text = post.Message ?? ""
                };

                var classificationResult = await _mediator.Send(ClassifyPostRequest, cancellationToken);

                if (!classificationResult.Success)
                {
                    _logger.LogWarning("Classificação falhou para o post {PostId}: {Error}", post.Id, classificationResult.Error);
                    continue; 
                }

                post.Topic = classificationResult?.Data?.Classification!;
            }

            await _facebookRepository.SavePostsAsync(postsUnicos);
            _logger.LogInformation("{Count} posts do Facebook salvos com sucesso", postsUnicos.Count);
            return Result<ProcessDatasetResponse>.Ok(new ProcessDatasetResponse() { Posts = postsUnicos });
        }
    }
}
