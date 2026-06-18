using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateStudentRequest
{
    [Required]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }
}

public class UpdateStudentRequest
{
    [Required]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }
}
