namespace PRN232.LMS.Services.BusinessModels;

public class EnrollmentSummaryBusinessModel
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateOnly EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public StudentSummaryBusinessModel? Student { get; set; }
    public CourseSummaryBusinessModel? Course { get; set; }
}
