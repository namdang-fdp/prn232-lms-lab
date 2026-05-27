using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public class SubjectService(
    IGenericRepository<Subject> subjectRepository,
    IGenericRepository<Course> courseRepository,
    IMapper mapper) : LmsServiceBase(mapper), ISubjectService
{
    public async Task<PagedResultBusinessModel<SubjectBusinessModel>> GetAsync(
        QueryParametersBusinessModel query,
        CancellationToken cancellationToken = default)
    {
        var result = await subjectRepository.GetPagedAsync(
            LmsQueryConfigurations.ForSubjects(query.Search, query.Sort, query.Expand, Page(query), Size(query)),
            cancellationToken);

        return ToPagedResult<Subject, SubjectBusinessModel>(result);
    }

    public async Task<SubjectBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetByIdAsync(
            entity => entity.SubjectId == id,
            LmsQueryConfigurations.SubjectDetails(),
            cancellationToken);

        return subject is null
            ? throw NotFound("Subject", id)
            : Mapper.Map<SubjectBusinessModel>(subject);
    }

    public async Task<SubjectBusinessModel> CreateAsync(SubjectBusinessModel model, CancellationToken cancellationToken = default)
    {
        await ValidateSubjectAsync(model, null, cancellationToken);

        var subject = Mapper.Map<Subject>(model);

        await subjectRepository.AddAsync(subject, cancellationToken);
        await subjectRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(subject.SubjectId, cancellationToken);
    }

    public async Task<SubjectBusinessModel> UpdateAsync(
        int id,
        SubjectBusinessModel model,
        CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetByIdAsync(
            entity => entity.SubjectId == id,
            cancellationToken: cancellationToken);

        if (subject is null)
        {
            throw NotFound("Subject", id);
        }

        model.SubjectId = id;
        await ValidateSubjectAsync(model, id, cancellationToken);

        subject.SubjectCode = model.SubjectCode.Trim().ToUpperInvariant();
        subject.SubjectName = model.SubjectName.Trim();
        subject.Credit = model.Credit;

        subjectRepository.Update(subject);
        await subjectRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetByIdAsync(
            entity => entity.SubjectId == id,
            cancellationToken: cancellationToken);

        if (subject is null)
        {
            throw NotFound("Subject", id);
        }

        if (await courseRepository.AnyAsync(course => course.SubjectId == id, cancellationToken))
        {
            throw Conflict("Cannot delete a subject that still has courses.");
        }

        subjectRepository.Delete(subject);
        await subjectRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateSubjectAsync(
        SubjectBusinessModel model,
        int? existingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.SubjectCode))
        {
            throw ValidationError("Subject code is required.", nameof(model.SubjectCode));
        }

        if (string.IsNullOrWhiteSpace(model.SubjectName))
        {
            throw ValidationError("Subject name is required.", nameof(model.SubjectName));
        }

        if (model.Credit <= 0)
        {
            throw ValidationError("Credit must be greater than zero.", nameof(model.Credit));
        }

        var normalizedCode = model.SubjectCode.Trim().ToUpperInvariant();
        var duplicateExists = await subjectRepository.AnyAsync(
            subject => subject.SubjectCode == normalizedCode && (!existingId.HasValue || subject.SubjectId != existingId.Value),
            cancellationToken);

        if (duplicateExists)
        {
            throw Duplicate($"Subject code '{normalizedCode}' already exists.");
        }

        model.SubjectCode = normalizedCode;
        model.SubjectName = model.SubjectName.Trim();
    }
}
