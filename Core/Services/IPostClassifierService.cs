namespace Core.Services
{
  public interface IPostClassifierService{
    Task<string> ClassifyPostAsync(string text);
  }
}