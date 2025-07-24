namespace Data
{
    public interface IYouTubeVideoRepository
    {
        Task AddOrUpdateAsync(Core.YouTubeVideo video);
        Task<IEnumerable<Core.YouTubeVideo>> GetLatestAsync(int count = 50);
    }
}
