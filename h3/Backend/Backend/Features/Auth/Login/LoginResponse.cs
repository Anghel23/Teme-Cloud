namespace Backend.Features.Auth.Login
{
    public record LoginResponse(string Token, Guid UserId, string Username, string Email);
}
