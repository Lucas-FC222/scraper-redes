using Data;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/youtube")]
    public class YouTubeController : Controller
    {

        private readonly IYouTubeAnalyzerService _analyzer;
        public YouTubeController(IYouTubeAnalyzerService analyzer) => _analyzer = analyzer;

        [HttpPost("analyze/{channel}")]
        public async Task<IActionResult> Analyze(string channel, [FromQuery] DateTime? since)
        {
            await _analyzer.AnalyzeChannelAsync(channel, since);
            return Ok();
        }

        [HttpGet("recent/{count?}")]
        public async Task<IActionResult> Recent([FromServices] IYouTubeVideoRepository repo, int count = 10)
            => Ok(await repo.GetLatestAsync(count));
    }
}


