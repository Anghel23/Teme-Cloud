using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Auth.Register
{
    [ApiController]
    [Route("v1/api")]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator mediator;

        public RegisterController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Created("/v1/register", result.Data);
        }
    }
}
