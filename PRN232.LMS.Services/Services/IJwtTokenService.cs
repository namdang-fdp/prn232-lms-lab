using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Services;

public interface IJwtTokenService
{
    int AccessTokenExpiresInSeconds { get; }
    int RefreshTokenDays { get; }
    string GenerateAccessToken(UserBusinessModel user);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
    bool VerifyRefreshTokenHash(string refreshToken, string refreshTokenHash);
}
