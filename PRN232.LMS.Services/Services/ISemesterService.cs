using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public interface ISemesterService
{
    Task<PagedResultBusinessModel<SemesterBusinessModel>> GetAsync(QueryParametersBusinessModel query, CancellationToken cancellationToken = default);
    Task<SemesterBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SemesterBusinessModel> CreateAsync(SemesterBusinessModel model, CancellationToken cancellationToken = default);
    Task<SemesterBusinessModel> UpdateAsync(int id, SemesterBusinessModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
