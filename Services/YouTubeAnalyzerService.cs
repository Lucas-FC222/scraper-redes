using Core;
using Data;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class YouTubeAnalyzerService : IYouTubeAnalyzerService
    {
        private readonly IYouTubeVideoRepository _repo;
        private readonly ILogger<YouTubeAnalyzerService> _logger;
        private readonly string _apiKey;

        public YouTubeAnalyzerService(IYouTubeVideoRepository repo, IConfiguration cfg, ILogger<YouTubeAnalyzerService> logger)
        {
            _repo = repo;
            _apiKey = cfg["YouTube:ApiKey"];
            _logger = logger;
        }

        public async Task AnalyzeChannelAsync(string channelName, DateTime? since)
        {
            var yt = new YouTubeService(new BaseClientService.Initializer { ApiKey = _apiKey });
            // 1) Encontrar channelId
            var search = yt.Search.List("snippet");
            search.Q = channelName;
            search.Type = "channel";
            var chRes = await search.ExecuteAsync();
            var channelId = chRes.Items.FirstOrDefault()?.Snippet.ChannelId;
            if (channelId == null) throw new Exception("Canal não encontrado");

            // 2) Listar vídeos
            var vids = new List<string>(); string next = null;
            do
            {
                var listReq = yt.Search.List("snippet");
                listReq.ChannelId = channelId;
                listReq.Order = SearchResource.ListRequest.OrderEnum.Date;
                listReq.MaxResults = 50;
                listReq.PageToken = next;
                var listRes = await listReq.ExecuteAsync();
                vids.AddRange(listRes.Items
                  .Where(i => !since.HasValue || i.Snippet.PublishedAt > since)
                  .Select(i => i.Id.VideoId));
                next = listRes.NextPageToken;
            } while (next != null);

            // 3) Obter detalhes em lotes de 50
            for (int i = 0; i < vids.Count; i += 50)
            {
                var batch = vids.Skip(i).Take(50);
                var detReq = yt.Videos.List("snippet,contentDetails,statistics");
                detReq.Id = string.Join(',', batch);
                var detRes = await detReq.ExecuteAsync();
                foreach (var item in detRes.Items)
                {
                    var v = new YouTubeVideo
                    {
                        VideoId = item.Id,
                        Title = item.Snippet.Title,
                        Description = item.Snippet.Description,
                        PublishedAt = item.Snippet.PublishedAt ?? DateTime.UtcNow,
                        ThumbnailUrl = item.Snippet.Thumbnails.Default__.Url,
                        ViewCount = (long)(item.Statistics.ViewCount ?? 0),
                        LikeCount = (long)(item.Statistics.LikeCount ?? 0),
                        CommentCount = (long)(item.Statistics.CommentCount ?? 0)
                    };
                    await _repo.AddOrUpdateAsync(v);
                }
            }
        }
    }
}
