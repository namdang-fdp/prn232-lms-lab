namespace PRN232.LMS.Services.Models;

public class SemesterModel : SemesterSummaryModel
{
    public List<CourseSummaryModel>? Courses { get; set; }
}
