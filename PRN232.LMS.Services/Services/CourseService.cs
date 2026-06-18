using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public class CourseService(
    IGenericRepository<Course> courseRepository,
    IGenericRepository<Semester> semesterRepository,
    IGenericRepository<Subject> subjectRepository,
    IGenericRepository<Enrollment> enrollmentRepository,
    IGenericRepository<Student> studentRepository,
    IMapper mapper) : LmsServiceBase(mapper), ICourseService
{
    public async Task<PagedResultBusinessModel<CourseBusinessModel>> GetAsync(
        QueryParametersBusinessModel query,
        CancellationToken cancellationToken = default)
    {
        var result = await courseRepository.GetPagedAsync(
            LmsQueryConfigurations.ForCourses(query.Search, query.Sort, query.Expand, Page(query), Size(query)),
            cancellationToken);

        return ToPagedResult<Course, CourseBusinessModel>(result);
    }

    public async Task<CourseBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(
            entity => entity.CourseId == id,
            LmsQueryConfigurations.CourseDetails(),
            cancellationToken);

        return course is null
            ? throw NotFound("Course", id)
            : Mapper.Map<CourseBusinessModel>(course);
    }

    public async Task<PagedResultBusinessModel<EnrollmentBusinessModel>> GetEnrollmentsAsync(
        int id,
        QueryParametersBusinessModel query,
        CancellationToken cancellationToken = default)
    {
        if (!await courseRepository.AnyAsync(course => course.CourseId == id, cancellationToken))
        {
            throw NotFound("Course", id);
        }

        var options = LmsQueryConfigurations.ForEnrollments(
            query.Search,
            query.Sort,
            query.Expand,
            Page(query),
            Size(query));
        options.Filter = enrollment => enrollment.CourseId == id;

        var result = await enrollmentRepository.GetPagedAsync(options, cancellationToken);

        return ToPagedResult<Enrollment, EnrollmentBusinessModel>(result);
    }

    public async Task<PagedResultBusinessModel<StudentBusinessModel>> GetStudentsAsync(
        int id,
        QueryParametersBusinessModel query,
        CancellationToken cancellationToken = default)
    {
        if (!await courseRepository.AnyAsync(course => course.CourseId == id, cancellationToken))
        {
            throw NotFound("Course", id);
        }

        var options = LmsQueryConfigurations.ForStudents(
            query.Search,
            query.Sort,
            query.Expand,
            Page(query),
            Size(query));
        options.Filter = student => student.Enrollments.Any(enrollment => enrollment.CourseId == id);

        var result = await studentRepository.GetPagedAsync(options, cancellationToken);

        return ToPagedResult<Student, StudentBusinessModel>(result);
    }

    public async Task<CourseBusinessModel> CreateAsync(CourseBusinessModel model, CancellationToken cancellationToken = default)
    {
        await ValidateCourseAsync(model, null, cancellationToken);

        var course = Mapper.Map<Course>(model);

        await courseRepository.AddAsync(course, cancellationToken);
        await courseRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(course.CourseId, cancellationToken);
    }

    public async Task<CourseBusinessModel> UpdateAsync(
        int id,
        CourseBusinessModel model,
        CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(
            entity => entity.CourseId == id,
            cancellationToken: cancellationToken);

        if (course is null)
        {
            throw NotFound("Course", id);
        }

        model.CourseId = id;
        await ValidateCourseAsync(model, id, cancellationToken);

        course.CourseName = model.CourseName.Trim();
        course.SemesterId = model.SemesterId;
        course.SubjectId = model.SubjectId;

        courseRepository.Update(course);
        await courseRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(
            entity => entity.CourseId == id,
            cancellationToken: cancellationToken);

        if (course is null)
        {
            throw NotFound("Course", id);
        }

        if (await enrollmentRepository.AnyAsync(enrollment => enrollment.CourseId == id, cancellationToken))
        {
            throw Conflict("Cannot delete a course that still has enrollments.");
        }

        courseRepository.Delete(course);
        await courseRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateCourseAsync(
        CourseBusinessModel model,
        int? existingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.CourseName))
        {
            throw ValidationError("Course name is required.", nameof(model.CourseName));
        }

        if (!await semesterRepository.AnyAsync(semester => semester.SemesterId == model.SemesterId, cancellationToken))
        {
            throw ValidationError("Semester does not exist.", nameof(model.SemesterId));
        }

        if (!await subjectRepository.AnyAsync(subject => subject.SubjectId == model.SubjectId, cancellationToken))
        {
            throw ValidationError("Subject does not exist.", nameof(model.SubjectId));
        }

        var normalizedName = model.CourseName.Trim();
        var duplicateExists = await courseRepository.AnyAsync(
            course =>
                course.CourseName == normalizedName &&
                course.SemesterId == model.SemesterId &&
                course.SubjectId == model.SubjectId &&
                (!existingId.HasValue || course.CourseId != existingId.Value),
            cancellationToken);

        if (duplicateExists)
        {
            throw Duplicate("Course already exists for the selected semester and subject.");
        }

        model.CourseName = normalizedName;
    }
}
