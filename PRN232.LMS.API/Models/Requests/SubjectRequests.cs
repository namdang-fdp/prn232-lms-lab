using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateSubjectRequest
{
    [Required]
    [StringLength(20)]
    [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Subject code can contain only letters and numbers.")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string SubjectName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credit { get; set; }
}

public class UpdateSubjectRequest
{
    [Required]
    [StringLength(20)]
    [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Subject code can contain only letters and numbers.")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string SubjectName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credit { get; set; }
}
