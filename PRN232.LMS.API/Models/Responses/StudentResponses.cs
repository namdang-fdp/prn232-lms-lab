using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Models.Responses;

public class StudentSummaryResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
}

public class StudentResponse : StudentSummaryResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<EnrollmentSummaryResponse>? Enrollments { get; set; }
}
