namespace PRN232.LMS.Services.BusinessModels;

public class SubjectBusinessModel : SubjectSummaryBusinessModel
{
    public List<CourseSummaryBusinessModel>? Courses { get; set; }
}
