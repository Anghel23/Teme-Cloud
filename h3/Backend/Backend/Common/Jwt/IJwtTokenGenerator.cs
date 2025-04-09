using Backend.Entities;

namespace Backend.Common.Jwt
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }

}
