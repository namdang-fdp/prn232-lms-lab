using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Models.Responses;

public class SubjectSummaryResponse
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }
}

public class SubjectResponse : SubjectSummaryResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CourseSummaryResponse>? Courses { get; set; }
}
