using System.Linq.Expressions;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Repositories.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<RepositoryPagedResult<TEntity>> GetPagedAsync(
        RepositoryQueryOptions<TEntity> options,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
