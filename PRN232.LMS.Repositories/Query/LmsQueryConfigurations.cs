using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Query;

public static class LmsQueryConfigurations
{
    public static RepositoryQueryOptions<Semester> ForSemesters(
        string? search,
        string? sort,
        string? expand,
        int page,
        int size)
    {
        return new RepositoryQueryOptions<Semester>
        {
            Search = search,
            Sort = string.IsNullOrWhiteSpace(sort) ? "semesterId" : sort,
            Expand = expand,
            Page = page,
            Size = size,
            SearchFields = new Expression<Func<Semester, string?>>[] { semester => semester.SemesterName },
            SortFields = new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
            {
                ["semesterId"] = (Expression<Func<Semester, int>>)(semester => semester.SemesterId),
                ["semesterName"] = (Expression<Func<Semester, string>>)(semester => semester.SemesterName),
                ["startDate"] = (Expression<Func<Semester, DateOnly>>)(semester => semester.StartDate),
                ["endDate"] = (Expression<Func<Semester, DateOnly>>)(semester => semester.EndDate)
            },
            Expanders = new Dictionary<string, Func<IQueryable<Semester>, IQueryable<Semester>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["courses"] = query => query
                    .Include(semester => semester.Courses)
                    .ThenInclude(course => course.Subject)
            }
        };
    }

    public static RepositoryQueryOptions<Subject> ForSubjects(
        string? search,
        string? sort,
        string? expand,
        int page,
        int size)
    {
        return new RepositoryQueryOptions<Subject>
        {
            Search = search,
            Sort = string.IsNullOrWhiteSpace(sort) ? "subjectId" : sort,
            Expand = expand,
            Page = page,
            Size = size,
            SearchFields = new Expression<Func<Subject, string?>>[]
            {
                (Subject subject) => subject.SubjectCode,
                subject => subject.SubjectName
            },
            SortFields = new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
            {
                ["subjectId"] = (Expression<Func<Subject, int>>)(subject => subject.SubjectId),
                ["subjectCode"] = (Expression<Func<Subject, string>>)(subject => subject.SubjectCode),
                ["subjectName"] = (Expression<Func<Subject, string>>)(subject => subject.SubjectName),
                ["credit"] = (Expression<Func<Subject, int>>)(subject => subject.Credit)
            },
            Expanders = new Dictionary<string, Func<IQueryable<Subject>, IQueryable<Subject>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["courses"] = query => query
                    .Include(subject => subject.Courses)
                    .ThenInclude(course => course.Semester)
            }
        };
    }

    public static RepositoryQueryOptions<Course> ForCourses(
        string? search,
        string? sort,
        string? expand,
        int page,
        int size)
    {
        return new RepositoryQueryOptions<Course>
        {
            Search = search,
            Sort = string.IsNullOrWhiteSpace(sort) ? "courseId" : sort,
            Expand = expand,
            Page = page,
            Size = size,
            SearchFields = new Expression<Func<Course, string?>>[] { course => course.CourseName },
            SortFields = new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
            {
                ["courseId"] = (Expression<Func<Course, int>>)(course => course.CourseId),
                ["courseName"] = (Expression<Func<Course, string>>)(course => course.CourseName),
                ["semesterId"] = (Expression<Func<Course, int>>)(course => course.SemesterId),
                ["subjectId"] = (Expression<Func<Course, int>>)(course => course.SubjectId)
            },
            Expanders = new Dictionary<string, Func<IQueryable<Course>, IQueryable<Course>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["semester"] = query => query.Include(course => course.Semester),
                ["subject"] = query => query.Include(course => course.Subject),
                ["enrollments"] = query => query
                    .Include(course => course.Enrollments)
                    .ThenInclude(enrollment => enrollment.Student)
            }
        };
    }

    public static RepositoryQueryOptions<Student> ForStudents(
        string? search,
        string? sort,
        string? expand,
        int page,
        int size)
    {
        return new RepositoryQueryOptions<Student>
        {
            Search = search,
            Sort = string.IsNullOrWhiteSpace(sort) ? "studentId" : sort,
            Expand = expand,
            Page = page,
            Size = size,
            SearchFields = new Expression<Func<Student, string?>>[]
            {
                (Student student) => student.FullName,
                student => student.Email
            },
            SortFields = new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
            {
                ["studentId"] = (Expression<Func<Student, int>>)(student => student.StudentId),
                ["fullName"] = (Expression<Func<Student, string>>)(student => student.FullName),
                ["email"] = (Expression<Func<Student, string>>)(student => student.Email),
                ["dateOfBirth"] = (Expression<Func<Student, DateOnly>>)(student => student.DateOfBirth)
            },
            Expanders = new Dictionary<string, Func<IQueryable<Student>, IQueryable<Student>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["enrollments"] = query => query
                    .Include(student => student.Enrollments)
                    .ThenInclude(enrollment => enrollment.Course!)
                    .ThenInclude(course => course.Subject),
                ["courses"] = query => query
                    .Include(student => student.Enrollments)
                    .ThenInclude(enrollment => enrollment.Course!)
                    .ThenInclude(course => course.Semester)
            }
        };
    }

    public static RepositoryQueryOptions<Enrollment> ForEnrollments(
        string? search,
        string? sort,
        string? expand,
        int page,
        int size)
    {
        return new RepositoryQueryOptions<Enrollment>
        {
            Search = search,
            Sort = string.IsNullOrWhiteSpace(sort) ? "enrollmentId" : sort,
            Expand = expand,
            Page = page,
            Size = size,
            SearchFields = new Expression<Func<Enrollment, string?>>[] { enrollment => enrollment.Status },
            SortFields = new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
            {
                ["enrollmentId"] = (Expression<Func<Enrollment, int>>)(enrollment => enrollment.EnrollmentId),
                ["studentId"] = (Expression<Func<Enrollment, int>>)(enrollment => enrollment.StudentId),
                ["courseId"] = (Expression<Func<Enrollment, int>>)(enrollment => enrollment.CourseId),
                ["enrollDate"] = (Expression<Func<Enrollment, DateOnly>>)(enrollment => enrollment.EnrollDate),
                ["status"] = (Expression<Func<Enrollment, string>>)(enrollment => enrollment.Status)
            },
            Expanders = new Dictionary<string, Func<IQueryable<Enrollment>, IQueryable<Enrollment>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["student"] = query => query.Include(enrollment => enrollment.Student),
                ["course"] = query => query
                    .Include(enrollment => enrollment.Course!)
                    .ThenInclude(course => course.Subject),
                ["semester"] = query => query
                    .Include(enrollment => enrollment.Course!)
                    .ThenInclude(course => course.Semester)
            }
        };
    }

    public static Func<IQueryable<Semester>, IQueryable<Semester>> SemesterDetails()
    {
        return query => query
            .Include(semester => semester.Courses)
            .ThenInclude(course => course.Subject);
    }

    public static Func<IQueryable<Subject>, IQueryable<Subject>> SubjectDetails()
    {
        return query => query
            .Include(subject => subject.Courses)
            .ThenInclude(course => course.Semester);
    }

    public static Func<IQueryable<Course>, IQueryable<Course>> CourseDetails()
    {
        return query => query
            .Include(course => course.Semester)
            .Include(course => course.Subject)
            .Include(course => course.Enrollments)
            .ThenInclude(enrollment => enrollment.Student);
    }

    public static Func<IQueryable<Student>, IQueryable<Student>> StudentDetails()
    {
        return query => query
            .Include(student => student.Enrollments)
            .ThenInclude(enrollment => enrollment.Course!)
            .ThenInclude(course => course.Subject)
            .Include(student => student.Enrollments)
            .ThenInclude(enrollment => enrollment.Course!)
            .ThenInclude(course => course.Semester);
    }

    public static Func<IQueryable<Enrollment>, IQueryable<Enrollment>> EnrollmentDetails()
    {
        return query => query
            .Include(enrollment => enrollment.Student)
            .Include(enrollment => enrollment.Course!)
            .ThenInclude(course => course.Subject)
            .Include(enrollment => enrollment.Course!)
            .ThenInclude(course => course.Semester);
    }
}
