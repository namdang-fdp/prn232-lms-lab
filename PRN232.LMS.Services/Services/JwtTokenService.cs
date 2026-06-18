using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Options;

namespace PRN232.LMS.Services.Services;

public class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private const int MinimumSecretBytes = 32;
    private readonly JwtOptions _options = options.Value;

    public int AccessTokenExpiresInSeconds => (int)TimeSpan.FromMinutes(_options.AccessTokenMinutes).TotalSeconds;
    public int RefreshTokenDays => _options.RefreshTokenDays;

    public string GenerateAccessToken(UserBusinessModel user)
    {
        var now = DateTime.UtcNow;
        var signingCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(
                JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string HashRefreshToken(string refreshToken)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(hashBytes);
    }

    public bool VerifyRefreshTokenHash(string refreshToken, string refreshTokenHash)
    {
        if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(refreshTokenHash))
        {
            return false;
        }

        var computedHash = HashRefreshToken(refreshToken);
        var computedBytes = Encoding.UTF8.GetBytes(computedHash);
        var storedBytes = Encoding.UTF8.GetBytes(refreshTokenHash);

        return computedBytes.Length == storedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(computedBytes, storedBytes);
    }

    private SymmetricSecurityKey GetSigningKey()
    {
        if (string.IsNullOrWhiteSpace(_options.Secret) ||
            Encoding.UTF8.GetByteCount(_options.Secret) < MinimumSecretBytes)
        {
            throw new InvalidOperationException("JWT signing secret is not configured correctly.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
    }
}
