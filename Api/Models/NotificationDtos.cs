using System;

namespace Api.Models
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public string PostId { get; set; } = "";
        public string Topic { get; set; } = "";
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public PostContentDto Content { get; set; } = null!;
    }

    public class PostContentDto
    {
        public string Id { get; set; } = "";
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public string? Url { get; set; }
        public string PostType { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class UpdatePreferencesDto
    {
        public string[] Topics { get; set; } = Array.Empty<string>();
    }
}
