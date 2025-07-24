using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YouTubeVideo
    {
        public int Id { get; set; }
        public string VideoId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime PublishedAt { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public string Url => $"https://www.youtube.com/watch?v={VideoId}";
        public long ViewCount { get; set; }
        public long LikeCount { get; set; }
        public long CommentCount { get; set; }
    }
}
