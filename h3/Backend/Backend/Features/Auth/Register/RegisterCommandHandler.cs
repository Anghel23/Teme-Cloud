using Backend.Common;
using Backend.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Backend.Features.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {
        private readonly AppDbContext dbContext;

        public RegisterCommandHandler(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (!EmailValidator.IsValidEmail(request.Email))
            {
                return Result<RegisterResponse>.Failure("Invalid email format");
            }

            var emailExists = await dbContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (emailExists)
            {
                return Result<RegisterResponse>.Failure("Email already exists");
            }

            var userExists = await dbContext.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
            if (userExists)
            {
                return Result<RegisterResponse>.Failure("Username already exists");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var response = new RegisterResponse(user.Id, user.Username);
            return Result<RegisterResponse>.Success(response);
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
