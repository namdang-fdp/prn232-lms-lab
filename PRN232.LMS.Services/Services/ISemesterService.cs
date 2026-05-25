using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public interface ISemesterService
{
    Task<PagedResultModel<SemesterModel>> GetAsync(QueryParametersModel query, CancellationToken cancellationToken = default);
    Task<SemesterModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SemesterModel> CreateAsync(SemesterModel model, CancellationToken cancellationToken = default);
    Task<SemesterModel> UpdateAsync(int id, SemesterModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
