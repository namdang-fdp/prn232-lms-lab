using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public class StudentService(
    IGenericRepository<Student> studentRepository,
    IGenericRepository<Enrollment> enrollmentRepository,
    IMapper mapper) : LmsServiceBase(mapper), IStudentService
{
    public async Task<PagedResultModel<StudentModel>> GetAsync(
        QueryParametersModel query,
        CancellationToken cancellationToken = default)
    {
        var result = await studentRepository.GetPagedAsync(
            LmsQueryConfigurations.ForStudents(query.Search, query.Sort, query.Expand, Page(query), Size(query)),
            cancellationToken);

        return ToPagedResult<Student, StudentModel>(result);
    }

    public async Task<StudentModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(
            entity => entity.StudentId == id,
            LmsQueryConfigurations.StudentDetails(),
            cancellationToken);

        return student is null
            ? throw NotFound("Student", id)
            : Mapper.Map<StudentModel>(student);
    }

    public async Task<StudentModel> CreateAsync(StudentModel model, CancellationToken cancellationToken = default)
    {
        await ValidateStudentAsync(model, null, cancellationToken);

        var student = Mapper.Map<Student>(model);

        await studentRepository.AddAsync(student, cancellationToken);
        await studentRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(student.StudentId, cancellationToken);
    }

    public async Task<StudentModel> UpdateAsync(
        int id,
        StudentModel model,
        CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(
            entity => entity.StudentId == id,
            cancellationToken: cancellationToken);

        if (student is null)
        {
            throw NotFound("Student", id);
        }

        model.StudentId = id;
        await ValidateStudentAsync(model, id, cancellationToken);

        student.FullName = model.FullName.Trim();
        student.Email = model.Email.Trim().ToLowerInvariant();
        student.DateOfBirth = model.DateOfBirth;

        studentRepository.Update(student);
        await studentRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(
            entity => entity.StudentId == id,
            cancellationToken: cancellationToken);

        if (student is null)
        {
            throw NotFound("Student", id);
        }

        if (await enrollmentRepository.AnyAsync(enrollment => enrollment.StudentId == id, cancellationToken))
        {
            throw Conflict("Cannot delete a student that still has enrollments.");
        }

        studentRepository.Delete(student);
        await studentRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateStudentAsync(
        StudentModel model,
        int? existingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            throw ValidationError("Full name is required.", nameof(model.FullName));
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            throw ValidationError("Email is required.", nameof(model.Email));
        }

        if (model.DateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw ValidationError("Date of birth must be in the past.", nameof(model.DateOfBirth));
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var duplicateExists = await studentRepository.AnyAsync(
            student => student.Email == normalizedEmail && (!existingId.HasValue || student.StudentId != existingId.Value),
            cancellationToken);

        if (duplicateExists)
        {
            throw Duplicate($"Student email '{normalizedEmail}' already exists.");
        }

        model.FullName = model.FullName.Trim();
        model.Email = normalizedEmail;
    }
}
