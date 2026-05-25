namespace PRN232.LMS.Services.Models;

public class SubjectModel : SubjectSummaryModel
{
    public List<CourseSummaryModel>? Courses { get; set; }
}
