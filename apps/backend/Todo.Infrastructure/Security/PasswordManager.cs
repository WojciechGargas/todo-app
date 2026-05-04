using Microsoft.AspNetCore.Identity;
using Todo.Application.Security;
using Todo.Core.Entities;

namespace Todo.Infrastructure.Security;

public class PasswordManager(IPasswordHasher<User> passwordHasher ) : IPasswordManager
{
    public string Secure(string password)
        => passwordHasher.HashPassword(null!,  password);

    public bool Validate(string password, string securePassword)
        => passwordHasher.VerifyHashedPassword(null!, securePassword, password)
            is PasswordVerificationResult.Success;
}