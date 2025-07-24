using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;

namespace Core
{
    public class Video
    {
        public int Id { get; set; }

        [Required]
        public string VideoId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime PublishedAt { get; set; }

        public string Thumbnail { get; set; }

        public string Duration { get; set; }

        public long ViewCount { get; set; }

        public long LikeCount { get; set; }

        public long CommentCount { get; set; }

        public string Url { get; set; }

        public int ChannelIdFk { get; set; }

        public Channel Channel { get; set; }

        public Transcription Transcription { get; set; }

    }
}
