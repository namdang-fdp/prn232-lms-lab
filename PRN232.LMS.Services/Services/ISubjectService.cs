using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public interface ISubjectService
{
    Task<PagedResultModel<SubjectModel>> GetAsync(QueryParametersModel query, CancellationToken cancellationToken = default);
    Task<SubjectModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SubjectModel> CreateAsync(SubjectModel model, CancellationToken cancellationToken = default);
    Task<SubjectModel> UpdateAsync(int id, SubjectModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
