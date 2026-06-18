using Microsoft.AspNetCore.Identity;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Services;

public class PasswordHasherService(IPasswordHasher<User> passwordHasher) : IPasswordHasherService
{
    public string HashPassword(User user, string password)
    {
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
