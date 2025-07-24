using Core;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class YouTubeVideoRepository : IYouTubeVideoRepository
    {
        private readonly SqlConnection _conn;
        private readonly ILogger<YouTubeVideoRepository> _logger;
        public YouTubeVideoRepository(SqlConnection conn, ILogger<YouTubeVideoRepository> logger)
        {
            _conn = conn;
            _logger = logger;
        }

        public async Task AddOrUpdateAsync(YouTubeVideo v)
        {
            const string sql = @"
MERGE INTO YouTubeVideos AS t
USING (VALUES(@VideoId,@Title,@Description,@PublishedAt,@ThumbnailUrl,@ViewCount,@LikeCount,@CommentCount))
  AS s(VideoId,Title,Description,PublishedAt,ThumbnailUrl,ViewCount,LikeCount,CommentCount)
  ON t.VideoId=s.VideoId
WHEN MATCHED THEN UPDATE SET
  Title=s.Title,Description=s.Description,PublishedAt=s.PublishedAt,ThumbnailUrl=s.ThumbnailUrl,
  ViewCount=s.ViewCount,LikeCount=s.LikeCount,CommentCount=s.CommentCount
WHEN NOT MATCHED THEN INSERT(VideoId,Title,Description,PublishedAt,ThumbnailUrl,ViewCount,LikeCount,CommentCount)
  VALUES(s.VideoId,s.Title,s.Description,s.PublishedAt,s.ThumbnailUrl,s.ViewCount,s.LikeCount,s.CommentCount);";
            _logger.LogInformation("Upserting video {VideoId}", v.VideoId);
            await _conn.ExecuteAsync(sql, v);
        }

        public Task<IEnumerable<YouTubeVideo>> GetLatestAsync(int count)
        {
            const string sql = @"SELECT TOP(@Count) * FROM YouTubeVideos ORDER BY PublishedAt DESC;";
            _logger.LogInformation("Fetching {Count} latest videos", count);
            return _conn.QueryAsync<YouTubeVideo>(sql, new { Count = count });
        }
    }
}
