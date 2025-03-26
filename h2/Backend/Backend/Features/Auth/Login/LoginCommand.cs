using Backend.Common;
using MediatR;

namespace Backend.Features.Auth.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
}
