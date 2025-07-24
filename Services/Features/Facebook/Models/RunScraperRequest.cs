using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Models
{
    public class RunScraperRequest : IRequest<Result<RunScraperResponse>>
    {
        public string PageUrl { get; set; } = string.Empty;
        public int MaxPosts { get; set; }
    }
}
