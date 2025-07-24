namespace Shared.Services
{
  public interface IPostClassifierService{
    Task<string> ClassifyPostAsync(string text);
  }
}