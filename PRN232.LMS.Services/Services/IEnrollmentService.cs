using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public interface IEnrollmentService
{
    Task<PagedResultBusinessModel<EnrollmentBusinessModel>> GetAsync(QueryParametersBusinessModel query, CancellationToken cancellationToken = default);
    Task<EnrollmentBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EnrollmentBusinessModel> CreateAsync(EnrollmentBusinessModel model, CancellationToken cancellationToken = default);
    Task<EnrollmentBusinessModel> UpdateAsync(int id, EnrollmentBusinessModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
