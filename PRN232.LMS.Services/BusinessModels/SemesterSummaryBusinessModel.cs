namespace PRN232.LMS.Services.BusinessModels;

public class SemesterSummaryBusinessModel
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
