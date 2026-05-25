using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Models.Responses;

public class CourseSummaryResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SemesterSummaryResponse? Semester { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SubjectSummaryResponse? Subject { get; set; }
}

public class CourseResponse : CourseSummaryResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<EnrollmentSummaryResponse>? Enrollments { get; set; }
}
