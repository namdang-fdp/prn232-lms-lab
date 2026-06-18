using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Exceptions;

namespace PRN232.LMS.Services.Services;

public class AuthService(
    IGenericRepository<User> userRepository,
    IGenericRepository<RefreshToken> refreshTokenRepository,
    IPasswordHasherService passwordHasherService,
    IJwtTokenService jwtTokenService,
    IMapper mapper) : IAuthService
{
    public async Task<UserBusinessModel?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = NormalizeUsername(username);

        if (string.IsNullOrWhiteSpace(normalizedUsername))
        {
            return null;
        }

        var user = await userRepository.GetByIdAsync(
            entity => entity.Username == normalizedUsername,
            cancellationToken: cancellationToken);

        return user is null ? null : mapper.Map<UserBusinessModel>(user);
    }

    public async Task<UserBusinessModel?> VerifyCredentialsAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = NormalizeUsername(username);

        if (string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await userRepository.GetByIdAsync(
            entity => entity.Username == normalizedUsername && entity.IsActive,
            cancellationToken: cancellationToken);

        if (user is null || !passwordHasherService.VerifyPassword(user, password))
        {
            return null;
        }

        return mapper.Map<UserBusinessModel>(user);
    }

    public async Task<AuthTokenBusinessModel> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = NormalizeUsername(username);

        if (string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(password))
        {
            throw Unauthorized();
        }

        var user = await userRepository.GetByIdAsync(
            entity => entity.Username == normalizedUsername && entity.IsActive,
            cancellationToken: cancellationToken);

        if (user is null || !passwordHasherService.VerifyPassword(user, password))
        {
            throw Unauthorized();
        }

        var userModel = mapper.Map<UserBusinessModel>(user);

        return await CreateTokenResponseAsync(user, userModel, cancellationToken);
    }

    public async Task<AuthTokenBusinessModel> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw InvalidRefreshToken();
        }

        var tokenHash = jwtTokenService.HashRefreshToken(refreshToken);
        var storedToken = await refreshTokenRepository.GetByIdAsync(
            entity => entity.TokenHash == tokenHash,
            query => query.Include(entity => entity.User),
            cancellationToken);

        if (storedToken is null ||
            storedToken.User is null ||
            !jwtTokenService.VerifyRefreshTokenHash(refreshToken, storedToken.TokenHash) ||
            storedToken.RevokedAt.HasValue ||
            storedToken.ExpiresAt <= DateTime.UtcNow ||
            !storedToken.User.IsActive)
        {
            throw InvalidRefreshToken();
        }

        var user = storedToken.User;
        var userModel = mapper.Map<UserBusinessModel>(user);

        storedToken.User = null;
        storedToken.RevokedAt = DateTime.UtcNow;
        refreshTokenRepository.Update(storedToken);

        return await CreateTokenResponseAsync(user, userModel, cancellationToken);
    }

    private static string NormalizeUsername(string username)
    {
        return username.Trim().ToLowerInvariant();
    }

    private async Task<AuthTokenBusinessModel> CreateTokenResponseAsync(
        User user,
        UserBusinessModel userModel,
        CancellationToken cancellationToken)
    {
        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = jwtTokenService.HashRefreshToken(refreshToken);
        var now = DateTime.UtcNow;

        await refreshTokenRepository.AddAsync(
            new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = refreshTokenHash,
                CreatedAt = now,
                ExpiresAt = now.AddDays(jwtTokenService.RefreshTokenDays)
            },
            cancellationToken);

        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthTokenBusinessModel
        {
            AccessToken = jwtTokenService.GenerateAccessToken(userModel),
            RefreshToken = refreshToken,
            ExpiresIn = jwtTokenService.AccessTokenExpiresInSeconds,
            Username = userModel.Username,
            Role = userModel.Role
        };
    }

    private static ServiceException Unauthorized()
    {
        return new ServiceException(
            ServiceErrorCode.Unauthorized,
            "Invalid username or password.",
            401);
    }

    private static ServiceException InvalidRefreshToken()
    {
        return new ServiceException(
            ServiceErrorCode.Unauthorized,
            "Invalid refresh token.",
            401);
    }
}
