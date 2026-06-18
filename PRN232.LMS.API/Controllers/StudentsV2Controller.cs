using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/students")]
[Produces("application/json", "application/xml")]
public class StudentsV2Controller(IStudentService studentService, IMapper mapper) : LmsControllerBase
{
    /// <summary>
    /// Gets a paged list of students for API version 2.
    /// </summary>
    /// <remarks>Version 2 is behavior-compatible with v1 and preserves search, sort, paging, field selection, and relationship expansion.</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetStudents(
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<QueryParametersBusinessModel>(request);
        var result = await studentService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<StudentResponse>>(result.Items);
        var page = ToPageResponse<StudentResponse, StudentBusinessModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Students retrieved successfully."));
    }
}
