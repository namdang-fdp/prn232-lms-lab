using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json", "application/xml")]
public class AuthController(IAuthService authService, IMapper mapper) : LmsControllerBase
{
    /// <summary>
    /// Authenticates a user and returns access and refresh tokens.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var token = await authService.LoginAsync(request.Username, request.Password, cancellationToken);
        var response = mapper.Map<AuthTokenResponse>(token);

        return Ok(new ApiResponse<AuthTokenResponse>(response, "Login successful."));
    }

    /// <summary>
    /// Rotates a refresh token and returns a new token pair.
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var token = await authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        var response = mapper.Map<AuthTokenResponse>(token);

        return Ok(new ApiResponse<AuthTokenResponse>(response, "Token refreshed successfully."));
    }

    /// <summary>
    /// Verifies that the current token belongs to an Admin user.
    /// </summary>
    [HttpGet("admin-check")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public ActionResult<ApiResponse<object?>> AdminCheck()
    {
        return Ok(new ApiResponse<object?>(null, "Admin authorization verified."));
    }
}
