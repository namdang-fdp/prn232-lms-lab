using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public class LmsDbContext(DbContextOptions<LmsDbContext> options) : DbContext(options)
{
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureSemester(modelBuilder);
        ConfigureSubject(modelBuilder);
        ConfigureCourse(modelBuilder);
        ConfigureStudent(modelBuilder);
        ConfigureEnrollment(modelBuilder);

        modelBuilder.Entity<Semester>().HasData(CreateSemesters());
        modelBuilder.Entity<Subject>().HasData(CreateSubjects());
        modelBuilder.Entity<Course>().HasData(CreateCourses());
        modelBuilder.Entity<Student>().HasData(CreateStudents());
        modelBuilder.Entity<Enrollment>().HasData(CreateEnrollments());
    }

    private static void ConfigureSemester(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(semester => semester.SemesterId);

            entity.Property(semester => semester.SemesterName)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(semester => semester.SemesterName)
                .IsUnique();
        });
    }

    private static void ConfigureSubject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(subject => subject.SubjectId);

            entity.Property(subject => subject.SubjectCode)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(subject => subject.SubjectName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(subject => subject.Credit)
                .IsRequired();

            entity.HasIndex(subject => subject.SubjectCode)
                .IsUnique();
        });
    }

    private static void ConfigureCourse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(course => course.CourseId);

            entity.Property(course => course.CourseName)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasOne(course => course.Semester)
                .WithMany(semester => semester.Courses)
                .HasForeignKey(course => course.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(course => course.Subject)
                .WithMany(subject => subject.Courses)
                .HasForeignKey(course => course.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(course => new { course.SemesterId, course.SubjectId, course.CourseName })
                .IsUnique();
        });
    }

    private static void ConfigureStudent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(student => student.StudentId);

            entity.Property(student => student.FullName)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(student => student.Email)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(student => student.Email)
                .IsUnique();
        });
    }

    private static void ConfigureEnrollment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(enrollment => enrollment.EnrollmentId);

            entity.Property(enrollment => enrollment.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasOne(enrollment => enrollment.Student)
                .WithMany(student => student.Enrollments)
                .HasForeignKey(enrollment => enrollment.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(enrollment => enrollment.Course)
                .WithMany(course => course.Enrollments)
                .HasForeignKey(enrollment => enrollment.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(enrollment => new { enrollment.StudentId, enrollment.CourseId })
                .IsUnique();
        });
    }

    private static IEnumerable<Semester> CreateSemesters()
    {
        return new[]
        {
            new Semester
            {
                SemesterId = 1,
                SemesterName = "Spring 2024",
                StartDate = new DateOnly(2024, 1, 8),
                EndDate = new DateOnly(2024, 5, 12)
            },
            new Semester
            {
                SemesterId = 2,
                SemesterName = "Summer 2024",
                StartDate = new DateOnly(2024, 5, 20),
                EndDate = new DateOnly(2024, 8, 25)
            },
            new Semester
            {
                SemesterId = 3,
                SemesterName = "Fall 2024",
                StartDate = new DateOnly(2024, 9, 2),
                EndDate = new DateOnly(2024, 12, 22)
            },
            new Semester
            {
                SemesterId = 4,
                SemesterName = "Spring 2025",
                StartDate = new DateOnly(2025, 1, 6),
                EndDate = new DateOnly(2025, 5, 11)
            },
            new Semester
            {
                SemesterId = 5,
                SemesterName = "Summer 2025",
                StartDate = new DateOnly(2025, 5, 19),
                EndDate = new DateOnly(2025, 8, 24)
            }
        };
    }

    private static IEnumerable<Subject> CreateSubjects()
    {
        return new[]
        {
            new Subject { SubjectId = 1, SubjectCode = "PRN232", SubjectName = "Advanced Cross-Platform Application Programming", Credit = 3 },
            new Subject { SubjectId = 2, SubjectCode = "PRO192", SubjectName = "Object-Oriented Programming", Credit = 3 },
            new Subject { SubjectId = 3, SubjectCode = "DBI202", SubjectName = "Database Systems", Credit = 3 },
            new Subject { SubjectId = 4, SubjectCode = "SWE201", SubjectName = "Introduction to Software Engineering", Credit = 3 },
            new Subject { SubjectId = 5, SubjectCode = "CSD201", SubjectName = "Data Structures and Algorithms", Credit = 3 },
            new Subject { SubjectId = 6, SubjectCode = "PRJ301", SubjectName = "Java Web Application Development", Credit = 3 },
            new Subject { SubjectId = 7, SubjectCode = "SWP391", SubjectName = "Application Development Project", Credit = 4 },
            new Subject { SubjectId = 8, SubjectCode = "MAS291", SubjectName = "Statistics and Probability", Credit = 3 },
            new Subject { SubjectId = 9, SubjectCode = "WED201", SubjectName = "Web Design", Credit = 3 },
            new Subject { SubjectId = 10, SubjectCode = "ITE302", SubjectName = "Ethics in IT", Credit = 2 }
        };
    }

    private static IEnumerable<Course> CreateCourses()
    {
        var courses = new List<Course>();

        for (var courseId = 1; courseId <= 20; courseId++)
        {
            var subjectId = ((courseId - 1) % 10) + 1;
            var semesterId = ((courseId - 1) % 5) + 1;
            var section = courseId <= 10 ? "A" : "B";

            courses.Add(new Course
            {
                CourseId = courseId,
                CourseName = $"Course {courseId:00} - Section {section}",
                SemesterId = semesterId,
                SubjectId = subjectId
            });
        }

        return courses;
    }

    private static IEnumerable<Student> CreateStudents()
    {
        var firstNames = new[]
        {
            "An", "Bao", "Chi", "Duc", "Giang", "Ha", "Khanh", "Linh", "Minh", "Nam"
        };
        var lastNames = new[]
        {
            "Nguyen", "Tran", "Le", "Pham", "Hoang"
        };

        var students = new List<Student>();

        for (var studentId = 1; studentId <= 50; studentId++)
        {
            var firstName = firstNames[(studentId - 1) % firstNames.Length];
            var lastName = lastNames[(studentId - 1) % lastNames.Length];
            var month = ((studentId - 1) % 12) + 1;
            var day = ((studentId - 1) % 28) + 1;

            students.Add(new Student
            {
                StudentId = studentId,
                FullName = $"{lastName} {firstName} {studentId:00}",
                Email = $"student{studentId:00}@lms.local",
                DateOfBirth = new DateOnly(2000 + (studentId % 5), month, day)
            });
        }

        return students;
    }

    private static IEnumerable<Enrollment> CreateEnrollments()
    {
        var statuses = new[] { "Active", "Completed", "Dropped", "Pending" };
        var enrollments = new List<Enrollment>();
        var enrollmentId = 1;

        for (var studentId = 1; studentId <= 50; studentId++)
        {
            for (var index = 0; index < 10; index++)
            {
                var courseId = ((studentId + index - 1) % 20) + 1;

                enrollments.Add(new Enrollment
                {
                    EnrollmentId = enrollmentId,
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollDate = new DateOnly(2024 + (courseId % 2), ((courseId - 1) % 12) + 1, ((index + studentId - 1) % 28) + 1),
                    Status = statuses[(studentId + courseId + index) % statuses.Length]
                });

                enrollmentId++;
            }
        }

        return enrollments;
    }
}
