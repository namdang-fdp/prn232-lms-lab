using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Extensions;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.API.Controllers;

[ApiController]
public abstract class LmsControllerBase : ControllerBase
{
    protected static PageResponse<object> ToPageResponse<TResponse, TModel>(
        PagedResultModel<TModel> result,
        IEnumerable<TResponse> responses,
        string? fields)
    {
        return new PageResponse<object>
        {
            Content = ResponseShaper.ShapeMany(responses, fields),
            Pagination = new PaginationMetadata
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems
            }
        };
    }
}
