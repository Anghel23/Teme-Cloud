using Backend.Common;
using MediatR;

namespace Backend.Features.Auth.Register
{
    public record RegisterCommand(string Username, string Email, string Password) : IRequest<Result<RegisterResponse>>;
}
