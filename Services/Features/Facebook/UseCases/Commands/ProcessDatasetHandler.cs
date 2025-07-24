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
    public class ProcessDatasetHandler : IRequestHandler<ProcessDatasetRequest, Result<ProcessDatasetResponse>>
    {
        private readonly IApifyFacebookClient _apifyFacebookClient;
        private readonly ILogger<ProcessDatasetHandler> _logger;
        private readonly IFacebookRepository _facebookRepository;

        public ProcessDatasetHandler(IApifyFacebookClient apifyFacebookClient, ILogger<ProcessDatasetHandler> logger, IFacebookRepository facebookRepository)
        {
            _apifyFacebookClient = apifyFacebookClient;
            _logger = logger;
            _facebookRepository = facebookRepository;
        }

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
                    Detail = "Nenhum post encontrado no dataset do Facebook: {request.DatasetId}",
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
                post.Topic = await _postClassifierService.ClassifyPostAsync(post.Message ?? "");
            }

            await _facebookRepository.SavePostsAsync(postsUnicos);
            _logger.LogInformation("{Count} posts do Facebook salvos com sucesso", postsUnicos.Count);
            return Result<ProcessDatasetResponse>.Ok(new ProcessDatasetResponse() { Posts = postsUnicos });
        }
    }
}
