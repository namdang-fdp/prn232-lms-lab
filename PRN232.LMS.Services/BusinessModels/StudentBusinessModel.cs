namespace PRN232.LMS.Services.BusinessModels;

public class StudentBusinessModel : StudentSummaryBusinessModel
{
    public List<EnrollmentSummaryBusinessModel>? Enrollments { get; set; }
}
