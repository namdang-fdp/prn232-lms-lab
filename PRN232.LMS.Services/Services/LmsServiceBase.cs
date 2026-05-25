using AutoMapper;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

public abstract class LmsServiceBase(IMapper mapper)
{
    protected IMapper Mapper { get; } = mapper;

    protected PagedResultModel<TModel> ToPagedResult<TEntity, TModel>(RepositoryPagedResult<TEntity> result)
    {
        return new PagedResultModel<TModel>
        {
            Items = Mapper.Map<IReadOnlyList<TModel>>(result.Items),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    protected static int Page(QueryParametersModel query)
    {
        return query.Page < 1 ? 1 : query.Page;
    }

    protected static int Size(QueryParametersModel query)
    {
        return query.Size switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => query.Size
        };
    }

    protected static ServiceException NotFound(string resourceName, int id)
    {
        return new ServiceException(
            ServiceErrorCode.ResourceNotFound,
            $"{resourceName} with id {id} was not found.",
            404);
    }

    protected static ServiceException ValidationError(string message, string field)
    {
        return new ServiceException(
            ServiceErrorCode.ValidationError,
            message,
            400,
            new Dictionary<string, string> { [field] = message });
    }

    protected static ServiceException Duplicate(string message)
    {
        return new ServiceException(ServiceErrorCode.DuplicateResource, message, 409);
    }

    protected static ServiceException Conflict(string message)
    {
        return new ServiceException(ServiceErrorCode.Conflict, message, 409);
    }
}
