using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class LoginRequest
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    [Required]
    [StringLength(500)]
    public string RefreshToken { get; set; } = string.Empty;
}
