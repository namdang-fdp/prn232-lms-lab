using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public class EnrollmentService(
    IGenericRepository<Enrollment> enrollmentRepository,
    IGenericRepository<Student> studentRepository,
    IGenericRepository<Course> courseRepository,
    IMapper mapper) : LmsServiceBase(mapper), IEnrollmentService
{
    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Active",
        "Completed",
        "Dropped",
        "Pending"
    };

    public async Task<PagedResultBusinessModel<EnrollmentBusinessModel>> GetAsync(
        QueryParametersBusinessModel query,
        CancellationToken cancellationToken = default)
    {
        var result = await enrollmentRepository.GetPagedAsync(
            LmsQueryConfigurations.ForEnrollments(query.Search, query.Sort, query.Expand, Page(query), Size(query)),
            cancellationToken);

        return ToPagedResult<Enrollment, EnrollmentBusinessModel>(result);
    }

    public async Task<EnrollmentBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(
            entity => entity.EnrollmentId == id,
            LmsQueryConfigurations.EnrollmentDetails(),
            cancellationToken);

        return enrollment is null
            ? throw NotFound("Enrollment", id)
            : Mapper.Map<EnrollmentBusinessModel>(enrollment);
    }

    public async Task<EnrollmentBusinessModel> CreateAsync(EnrollmentBusinessModel model, CancellationToken cancellationToken = default)
    {
        await ValidateEnrollmentAsync(model, null, cancellationToken);

        var enrollment = Mapper.Map<Enrollment>(model);

        await enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await enrollmentRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(enrollment.EnrollmentId, cancellationToken);
    }

    public async Task<EnrollmentBusinessModel> UpdateAsync(
        int id,
        EnrollmentBusinessModel model,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(
            entity => entity.EnrollmentId == id,
            cancellationToken: cancellationToken);

        if (enrollment is null)
        {
            throw NotFound("Enrollment", id);
        }

        model.EnrollmentId = id;
        await ValidateEnrollmentAsync(model, id, cancellationToken);

        enrollment.StudentId = model.StudentId;
        enrollment.CourseId = model.CourseId;
        enrollment.EnrollDate = model.EnrollDate;
        enrollment.Status = NormalizeStatus(model.Status);

        enrollmentRepository.Update(enrollment);
        await enrollmentRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(
            entity => entity.EnrollmentId == id,
            cancellationToken: cancellationToken);

        if (enrollment is null)
        {
            throw NotFound("Enrollment", id);
        }

        enrollmentRepository.Delete(enrollment);
        await enrollmentRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateEnrollmentAsync(
        EnrollmentBusinessModel model,
        int? existingId,
        CancellationToken cancellationToken)
    {
        if (!await studentRepository.AnyAsync(student => student.StudentId == model.StudentId, cancellationToken))
        {
            throw ValidationError("Student does not exist.", nameof(model.StudentId));
        }

        if (!await courseRepository.AnyAsync(course => course.CourseId == model.CourseId, cancellationToken))
        {
            throw ValidationError("Course does not exist.", nameof(model.CourseId));
        }

        if (model.EnrollDate == default)
        {
            throw ValidationError("Enroll date is required.", nameof(model.EnrollDate));
        }

        if (string.IsNullOrWhiteSpace(model.Status) || !ValidStatuses.Contains(model.Status.Trim()))
        {
            throw ValidationError("Status must be Active, Completed, Dropped, or Pending.", nameof(model.Status));
        }

        var duplicateExists = await enrollmentRepository.AnyAsync(
            enrollment =>
                enrollment.StudentId == model.StudentId &&
                enrollment.CourseId == model.CourseId &&
                (!existingId.HasValue || enrollment.EnrollmentId != existingId.Value),
            cancellationToken);

        if (duplicateExists)
        {
            throw Duplicate("Student is already enrolled in this course.");
        }

        model.Status = NormalizeStatus(model.Status);
    }

    private static string NormalizeStatus(string status)
    {
        return ValidStatuses.First(validStatus => validStatus.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
