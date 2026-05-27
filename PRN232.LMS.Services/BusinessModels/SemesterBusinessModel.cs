namespace PRN232.LMS.Services.BusinessModels;

public class SemesterBusinessModel : SemesterSummaryBusinessModel
{
    public List<CourseSummaryBusinessModel>? Courses { get; set; }
}
