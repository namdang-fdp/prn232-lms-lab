using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateSemesterRequest
{
    [Required]
    [StringLength(100)]
    public string SemesterName { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}

public class UpdateSemesterRequest
{
    [Required]
    [StringLength(100)]
    public string SemesterName { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
