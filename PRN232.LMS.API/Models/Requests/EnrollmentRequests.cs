using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateEnrollmentRequest
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateOnly EnrollDate { get; set; }

    [Required]
    [StringLength(30)]
    [RegularExpression("^(Active|Completed|Dropped|Pending)$", ErrorMessage = "Status must be Active, Completed, Dropped, or Pending.")]
    public string Status { get; set; } = string.Empty;
}

public class UpdateEnrollmentRequest
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateOnly EnrollDate { get; set; }

    [Required]
    [StringLength(30)]
    [RegularExpression("^(Active|Completed|Dropped|Pending)$", ErrorMessage = "Status must be Active, Completed, Dropped, or Pending.")]
    public string Status { get; set; } = string.Empty;
}
