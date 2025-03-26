using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Favorite
{
    [ApiController]
    [Route("api/favorites")]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IMediator mediator;

        public FavoriteController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFavorite([FromQuery] Guid userId, [FromQuery] string rawgId)
        {
            var command = new DeleteFavoriteCommand(userId, rawgId);
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFavorites([FromQuery] Guid userId)
        {
            var query = new GetAllFavoritesQuery(userId);
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {   
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}

