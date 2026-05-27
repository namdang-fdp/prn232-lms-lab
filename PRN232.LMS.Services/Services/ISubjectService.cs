using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public interface ISubjectService
{
    Task<PagedResultBusinessModel<SubjectBusinessModel>> GetAsync(QueryParametersBusinessModel query, CancellationToken cancellationToken = default);
    Task<SubjectBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SubjectBusinessModel> CreateAsync(SubjectBusinessModel model, CancellationToken cancellationToken = default);
    Task<SubjectBusinessModel> UpdateAsync(int id, SubjectBusinessModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
