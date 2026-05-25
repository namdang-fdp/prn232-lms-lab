using Microsoft.Extensions.DependencyInjection;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Extensions;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLmsApplication(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddLmsRepositories(connectionString);

        services.AddScoped<ISemesterService, SemesterService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        return services;
    }

    public static void InitializeLmsDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();

        dbContext.Database.EnsureCreated();
    }
}
