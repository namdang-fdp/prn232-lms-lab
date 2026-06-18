using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Services;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password);
}
