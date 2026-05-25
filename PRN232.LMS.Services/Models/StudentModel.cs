namespace PRN232.LMS.Services.Models;

public class StudentModel : StudentSummaryModel
{
    public List<EnrollmentSummaryModel>? Enrollments { get; set; }
}
