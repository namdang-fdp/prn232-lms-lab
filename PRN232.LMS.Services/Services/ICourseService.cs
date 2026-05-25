using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public interface ICourseService
{
    Task<PagedResultModel<CourseModel>> GetAsync(QueryParametersModel query, CancellationToken cancellationToken = default);
    Task<CourseModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseModel> CreateAsync(CourseModel model, CancellationToken cancellationToken = default);
    Task<CourseModel> UpdateAsync(int id, CourseModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
