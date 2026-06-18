using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Services;

public interface IAuthService
{
    Task<UserBusinessModel?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserBusinessModel?> VerifyCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<AuthTokenBusinessModel> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<AuthTokenBusinessModel> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
