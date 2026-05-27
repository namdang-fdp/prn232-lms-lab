using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public class SemesterService(
    IGenericRepository<Semester> semesterRepository,
    IGenericRepository<Course> courseRepository,
    IMapper mapper) : LmsServiceBase(mapper), ISemesterService
{
    public async Task<PagedResultBusinessModel<SemesterBusinessModel>> GetAsync(
        QueryParametersBusinessModel query,
        CancellationToken cancellationToken = default)
    {
        var result = await semesterRepository.GetPagedAsync(
            LmsQueryConfigurations.ForSemesters(query.Search, query.Sort, query.Expand, Page(query), Size(query)),
            cancellationToken);

        return ToPagedResult<Semester, SemesterBusinessModel>(result);
    }

    public async Task<SemesterBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var semester = await semesterRepository.GetByIdAsync(
            entity => entity.SemesterId == id,
            LmsQueryConfigurations.SemesterDetails(),
            cancellationToken);

        return semester is null
            ? throw NotFound("Semester", id)
            : Mapper.Map<SemesterBusinessModel>(semester);
    }

    public async Task<SemesterBusinessModel> CreateAsync(SemesterBusinessModel model, CancellationToken cancellationToken = default)
    {
        await ValidateSemesterAsync(model, null, cancellationToken);

        var semester = Mapper.Map<Semester>(model);

        await semesterRepository.AddAsync(semester, cancellationToken);
        await semesterRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(semester.SemesterId, cancellationToken);
    }

    public async Task<SemesterBusinessModel> UpdateAsync(
        int id,
        SemesterBusinessModel model,
        CancellationToken cancellationToken = default)
    {
        var semester = await semesterRepository.GetByIdAsync(
            entity => entity.SemesterId == id,
            cancellationToken: cancellationToken);

        if (semester is null)
        {
            throw NotFound("Semester", id);
        }

        model.SemesterId = id;
        await ValidateSemesterAsync(model, id, cancellationToken);

        semester.SemesterName = model.SemesterName.Trim();
        semester.StartDate = model.StartDate;
        semester.EndDate = model.EndDate;

        semesterRepository.Update(semester);
        await semesterRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var semester = await semesterRepository.GetByIdAsync(
            entity => entity.SemesterId == id,
            cancellationToken: cancellationToken);

        if (semester is null)
        {
            throw NotFound("Semester", id);
        }

        if (await courseRepository.AnyAsync(course => course.SemesterId == id, cancellationToken))
        {
            throw Conflict("Cannot delete a semester that still has courses.");
        }

        semesterRepository.Delete(semester);
        await semesterRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateSemesterAsync(
        SemesterBusinessModel model,
        int? existingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.SemesterName))
        {
            throw ValidationError("Semester name is required.", nameof(model.SemesterName));
        }

        if (model.EndDate < model.StartDate)
        {
            throw ValidationError("Semester end date must be on or after start date.", nameof(model.EndDate));
        }

        var normalizedName = model.SemesterName.Trim();
        var duplicateExists = await semesterRepository.AnyAsync(
            semester => semester.SemesterName == normalizedName && (!existingId.HasValue || semester.SemesterId != existingId.Value),
            cancellationToken);

        if (duplicateExists)
        {
            throw Duplicate($"Semester '{normalizedName}' already exists.");
        }

        model.SemesterName = normalizedName;
    }
}
