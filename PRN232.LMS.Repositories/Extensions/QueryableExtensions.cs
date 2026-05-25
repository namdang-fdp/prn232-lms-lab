using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Repositories.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> ApplyQuery<TEntity>(
        this IQueryable<TEntity> query,
        RepositoryQueryOptions<TEntity> options)
        where TEntity : class
    {
        query = query.ApplyExpansion(options.Expand, options.Expanders);
        query = query.ApplySearch(options.Search, options.SearchFields);
        query = query.ApplySorting(options.Sort, options.SortFields);

        return query;
    }

    private static IQueryable<TEntity> ApplyExpansion<TEntity>(
        this IQueryable<TEntity> query,
        string? expand,
        IReadOnlyDictionary<string, Func<IQueryable<TEntity>, IQueryable<TEntity>>> expanders)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(expand))
        {
            return query;
        }

        foreach (var field in SplitCsv(expand))
        {
            if (!expanders.TryGetValue(field, out var expander))
            {
                continue;
            }

            query = expander(query);
        }

        return query;
    }

    private static IQueryable<TEntity> ApplySearch<TEntity>(
        this IQueryable<TEntity> query,
        string? search,
        IReadOnlyList<Expression<Func<TEntity, string?>>> searchFields)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(search) || searchFields.Count == 0)
        {
            return query;
        }

        var normalizedSearch = search.Trim().ToLower();
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        Expression? combinedExpression = null;

        foreach (var searchField in searchFields)
        {
            var fieldExpression = ReplaceParameter(searchField.Body, searchField.Parameters[0], parameter);
            var notNullExpression = Expression.NotEqual(fieldExpression, Expression.Constant(null, typeof(string)));
            var toLowerExpression = Expression.Call(fieldExpression, nameof(string.ToLower), Type.EmptyTypes);
            var containsExpression = Expression.Call(
                toLowerExpression,
                nameof(string.Contains),
                Type.EmptyTypes,
                Expression.Constant(normalizedSearch));
            var safeContainsExpression = Expression.AndAlso(notNullExpression, containsExpression);

            combinedExpression = combinedExpression is null
                ? safeContainsExpression
                : Expression.OrElse(combinedExpression, safeContainsExpression);
        }

        if (combinedExpression is null)
        {
            return query;
        }

        return query.Where(Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter));
    }

    private static IQueryable<TEntity> ApplySorting<TEntity>(
        this IQueryable<TEntity> query,
        string? sort,
        IReadOnlyDictionary<string, LambdaExpression> sortFields)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query;
        }

        IOrderedQueryable<TEntity>? orderedQuery = null;

        foreach (var sortToken in SplitCsv(sort))
        {
            var descending = sortToken.Length > 0 && sortToken[0] == '-';
            var fieldName = descending ? sortToken[1..] : sortToken;

            if (string.IsNullOrWhiteSpace(fieldName) || !sortFields.TryGetValue(fieldName, out var selector))
            {
                continue;
            }

            orderedQuery = ApplyOrdering(query, orderedQuery, selector, descending);
        }

        return orderedQuery ?? query;
    }

    public static async Task<RepositoryPagedResult<TEntity>> ToPagedResultAsync<TEntity>(
        this IQueryable<TEntity> query,
        int page,
        int size,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedSize = size switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => size
        };

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync(cancellationToken);

        return new RepositoryPagedResult<TEntity>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems
        };
    }

    private static IOrderedQueryable<TEntity> ApplyOrdering<TEntity>(
        IQueryable<TEntity> query,
        IOrderedQueryable<TEntity>? orderedQuery,
        LambdaExpression selector,
        bool descending)
    {
        var methodName = (orderedQuery, descending) switch
        {
            (null, false) => nameof(Queryable.OrderBy),
            (null, true) => nameof(Queryable.OrderByDescending),
            (_, false) => nameof(Queryable.ThenBy),
            (_, true) => nameof(Queryable.ThenByDescending)
        };

        var source = (object?)orderedQuery ?? query;
        var orderedMethod = GetQueryableOrderingMethod(methodName)
            .MakeGenericMethod(typeof(TEntity), selector.ReturnType);

        return (IOrderedQueryable<TEntity>)orderedMethod.Invoke(null, new[] { source, selector })!;
    }

    private static MethodInfo GetQueryableOrderingMethod(string methodName)
    {
        return typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method =>
                method.Name == methodName &&
                method.GetParameters().Length == 2);
    }

    private static IEnumerable<string> SplitCsv(string value)
    {
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item));
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression source, ParameterExpression target)
    {
        return new ParameterReplaceVisitor(source, target).Visit(expression) ?? expression;
    }

    private sealed class ParameterReplaceVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == source ? target : base.VisitParameter(node);
        }
    }
}
