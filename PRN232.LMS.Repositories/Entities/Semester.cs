namespace PRN232.LMS.Repositories.Entities;

public class Semester
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
