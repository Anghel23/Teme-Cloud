using Backend.Common;
using Backend.Common.Jwt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Backend.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly AppDbContext appDbContext;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public LoginCommandHandler(AppDbContext appDbContext, IJwtTokenGenerator jwtTokenGenerator)
        {
            this.appDbContext = appDbContext;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (!EmailValidator.IsValidEmail(request.Email))
            {
                return Result<LoginResponse>.Failure("Invalid email format");
            }

            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user == null)
            {
                return Result<LoginResponse>.Failure("Invalid email or password");
            }

            var passwordHash = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordHash)
            {
                return Result<LoginResponse>.Failure("Invalid email or password");
            }

            var token = jwtTokenGenerator.GenerateToken(user);

            var response = new LoginResponse(
                token,
                user.Id,
                user.Username,
                user.Email
            );

            return Result<LoginResponse>.Success(response);
        }

        private static class EmailValidator
        {
            private static readonly Regex EmailRegex = new Regex(
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            public static bool IsValidEmail(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                return EmailRegex.IsMatch(email);
            }
        }
    }
}
