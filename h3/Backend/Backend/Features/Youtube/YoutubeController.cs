using Backend.Common.APIKeys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backend.Features.Steam
{
    [ApiController]
    [Route("api/youtube")]
    public class YoutubeController : ControllerBase
    {
        private readonly YoutubeSettings _settings;
        private readonly HttpClient _httpClient;

        public YoutubeController(IOptions<YoutubeSettings> settings)
        {
            _settings = settings.Value;
            _httpClient = new HttpClient();
        }

        [HttpGet("trailer")]
        public async Task<IActionResult> GetTrailer([FromQuery] string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return BadRequest("Missing game name");

            var query = $"{gameName} official trailer";
            var requestUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(query)}&type=video&maxResults=1&key={_settings.ApiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "YouTube API call failed");

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            var item = parsed.RootElement
                .GetProperty("items")[0];

            var videoId = item.GetProperty("id").GetProperty("videoId").GetString();
            var title = item.GetProperty("snippet").GetProperty("title").GetString();
            var thumbnail = item.GetProperty("snippet").GetProperty("thumbnails").GetProperty("high").GetProperty("url").GetString();

            return Ok(new
            {
                videoId,
                title,
                thumbnail
            });
        }
    }
}
