using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Extensions;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.API.Controllers;

[ApiController]
public abstract class LmsControllerBase : ControllerBase
{
    protected NotFoundObjectResult NotFoundResponse(ServiceException exception)
    {
        return NotFound(new ApiResponse<object>
        {
            Success = false,
            Message = exception.Message,
            Data = null,
            Errors = exception.Errors
        });
    }

    protected static PageResponse<object> ToPageResponse<TResponse, TBusinessModel>(
        PagedResultBusinessModel<TBusinessModel> result,
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
