using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public interface IStudentService
{
    Task<PagedResultModel<StudentModel>> GetAsync(QueryParametersModel query, CancellationToken cancellationToken = default);
    Task<StudentModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentModel> CreateAsync(StudentModel model, CancellationToken cancellationToken = default);
    Task<StudentModel> UpdateAsync(int id, StudentModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
