namespace PRN232.LMS.Services.BusinessModels;

public class CourseBusinessModel : CourseSummaryBusinessModel
{
    public List<EnrollmentSummaryBusinessModel>? Enrollments { get; set; }
}
