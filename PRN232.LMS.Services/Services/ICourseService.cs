using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.Services.Services;

public interface ICourseService
{
    Task<PagedResultBusinessModel<CourseBusinessModel>> GetAsync(QueryParametersBusinessModel query, CancellationToken cancellationToken = default);
    Task<PagedResultBusinessModel<EnrollmentBusinessModel>> GetEnrollmentsAsync(int id, QueryParametersBusinessModel query, CancellationToken cancellationToken = default);
    Task<PagedResultBusinessModel<StudentBusinessModel>> GetStudentsAsync(int id, QueryParametersBusinessModel query, CancellationToken cancellationToken = default);
    Task<CourseBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseBusinessModel> CreateAsync(CourseBusinessModel model, CancellationToken cancellationToken = default);
    Task<CourseBusinessModel> UpdateAsync(int id, CourseBusinessModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
