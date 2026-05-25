namespace PRN232.LMS.Services.Models;

public class CourseModel : CourseSummaryModel
{
    public List<EnrollmentSummaryModel>? Enrollments { get; set; }
}
