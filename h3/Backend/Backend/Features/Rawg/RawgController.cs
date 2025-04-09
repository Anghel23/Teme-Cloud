using Backend.Common.APIKeys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Features.Platform
{
    [ApiController]
    [Route("api/rawg")]
    public class RawgController : ControllerBase
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public RawgController(IHttpClientFactory httpClientFactory, IOptions<RawgSettings> rawgOptions)
        {
            this.httpClient = httpClientFactory.CreateClient();
            this.apiKey = rawgOptions.Value.ApiKey;
        }

        [HttpGet("games")]
        public async Task<IActionResult> GetGames([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string search = "")
        {
            var endpoint = $"https://api.rawg.io/api/games?key={apiKey}&page={page}&page_size={pageSize}";

            if (!string.IsNullOrWhiteSpace(search))
                endpoint += $"&search={Uri.EscapeDataString(search)}";

            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            return Ok(data);
        }

        [HttpGet("games/{id}")]
        public async Task<IActionResult> GetGameById(string id)
        {
            var endpoint = $"https://api.rawg.io/api/games/{id}?key={apiKey}";

            var response = await httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var data = await response.Content.ReadAsStringAsync();
            return Ok(data);
        }
    }

}
