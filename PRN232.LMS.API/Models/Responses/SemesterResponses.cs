using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Models.Responses;

public class SemesterSummaryResponse
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

public class SemesterResponse : SemesterSummaryResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CourseSummaryResponse>? Courses { get; set; }
}
