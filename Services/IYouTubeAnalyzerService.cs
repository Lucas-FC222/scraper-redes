
namespace Services
{
    public interface IYouTubeAnalyzerService
    {
        Task AnalyzeChannelAsync(string channelName, DateTime? since = null);
    }
}
