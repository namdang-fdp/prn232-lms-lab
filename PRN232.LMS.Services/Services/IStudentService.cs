using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public interface IStudentService
{
    Task<PagedResultBusinessModel<StudentBusinessModel>> GetAsync(QueryParametersBusinessModel query, CancellationToken cancellationToken = default);
    Task<StudentBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentBusinessModel> CreateAsync(StudentBusinessModel model, CancellationToken cancellationToken = default);
    Task<StudentBusinessModel> UpdateAsync(int id, StudentBusinessModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
