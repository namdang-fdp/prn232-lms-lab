namespace PRN232.LMS.Services.Models;

public class SemesterSummaryModel
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
