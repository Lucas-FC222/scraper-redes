namespace Api.Models
{
    public class ApifyWebhookPayload
    {
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? EventType { get; set; }
        public WebhookEventData? EventData { get; set; }
        public System.Text.Json.JsonElement Resource { get; set; }
    }

    public class WebhookEventData
    {
        public string? ActorId { get; set; }
        public string? ActorRunId { get; set; }
    }
} 