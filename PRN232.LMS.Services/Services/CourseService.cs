using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public class CourseService(
    IGenericRepository<Course> courseRepository,
    IGenericRepository<Semester> semesterRepository,
    IGenericRepository<Subject> subjectRepository,
    IGenericRepository<Enrollment> enrollmentRepository,
    IMapper mapper) : LmsServiceBase(mapper), ICourseService
{
    public async Task<PagedResultModel<CourseModel>> GetAsync(
        QueryParametersModel query,
        CancellationToken cancellationToken = default)
    {
        var result = await courseRepository.GetPagedAsync(
            LmsQueryConfigurations.ForCourses(query.Search, query.Sort, query.Expand, Page(query), Size(query)),
            cancellationToken);

        return ToPagedResult<Course, CourseModel>(result);
    }

    public async Task<CourseModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(
            entity => entity.CourseId == id,
            LmsQueryConfigurations.CourseDetails(),
            cancellationToken);

        return course is null
            ? throw NotFound("Course", id)
            : Mapper.Map<CourseModel>(course);
    }

    public async Task<CourseModel> CreateAsync(CourseModel model, CancellationToken cancellationToken = default)
    {
        await ValidateCourseAsync(model, null, cancellationToken);

        var course = Mapper.Map<Course>(model);

        await courseRepository.AddAsync(course, cancellationToken);
        await courseRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(course.CourseId, cancellationToken);
    }

    public async Task<CourseModel> UpdateAsync(
        int id,
        CourseModel model,
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
        CourseModel model,
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
