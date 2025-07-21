using System.Text.Json.Serialization;

namespace Infra
{
    public class FacebookPost
    {
        [JsonPropertyName("id")]
        public required string PostId { get; set; }

    }
}
