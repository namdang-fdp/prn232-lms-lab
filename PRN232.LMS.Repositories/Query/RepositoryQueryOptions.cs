using System.Linq.Expressions;

namespace PRN232.LMS.Repositories.Query;

public class RepositoryQueryOptions<TEntity> where TEntity : class
{
    public Expression<Func<TEntity, bool>>? Filter { get; set; }
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public string? Expand { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public IReadOnlyList<Expression<Func<TEntity, string?>>> SearchFields { get; set; } =
        Array.Empty<Expression<Func<TEntity, string?>>>();
    public IReadOnlyDictionary<string, LambdaExpression> SortFields { get; set; } =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, Func<IQueryable<TEntity>, IQueryable<TEntity>>> Expanders { get; set; } =
        new Dictionary<string, Func<IQueryable<TEntity>, IQueryable<TEntity>>>(StringComparer.OrdinalIgnoreCase);
}
