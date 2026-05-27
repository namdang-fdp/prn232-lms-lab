namespace PRN232.LMS.Services.BusinessModels;

public class CourseSummaryBusinessModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }
    public SemesterSummaryBusinessModel? Semester { get; set; }
    public SubjectSummaryBusinessModel? Subject { get; set; }
}
