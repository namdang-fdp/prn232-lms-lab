using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Models.Responses;

public class EnrollmentSummaryResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateOnly EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StudentSummaryResponse? Student { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CourseSummaryResponse? Course { get; set; }
}

public class EnrollmentResponse : EnrollmentSummaryResponse
{
}
