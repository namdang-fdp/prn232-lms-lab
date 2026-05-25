namespace PRN232.LMS.Repositories.Query;

public class RepositoryPagedResult<TEntity>
{
    public IReadOnlyList<TEntity> Items { get; set; } = Array.Empty<TEntity>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
}
