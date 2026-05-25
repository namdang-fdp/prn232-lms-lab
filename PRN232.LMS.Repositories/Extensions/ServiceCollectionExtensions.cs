using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Repositories;

namespace PRN232.LMS.Repositories.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLmsRepositories(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<LmsDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
