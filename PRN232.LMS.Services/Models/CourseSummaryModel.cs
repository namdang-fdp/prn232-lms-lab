namespace PRN232.LMS.Services.Models;

public class CourseSummaryModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }
    public SemesterSummaryModel? Semester { get; set; }
    public SubjectSummaryModel? Subject { get; set; }
}
