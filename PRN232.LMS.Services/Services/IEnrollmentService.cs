using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public interface IEnrollmentService
{
    Task<PagedResultModel<EnrollmentModel>> GetAsync(QueryParametersModel query, CancellationToken cancellationToken = default);
    Task<EnrollmentModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EnrollmentModel> CreateAsync(EnrollmentModel model, CancellationToken cancellationToken = default);
    Task<EnrollmentModel> UpdateAsync(int id, EnrollmentModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
