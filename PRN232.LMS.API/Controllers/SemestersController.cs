using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[Route("api/semesters")]
[Produces("application/json")]
public class SemestersController(ISemesterService semesterService, IMapper mapper) : LmsControllerBase
{
    /// <summary>
    /// Gets a paged list of semesters.
    /// </summary>
    /// <remarks>Supports search, sort, paging, field selection, and relationship expansion.</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetSemesters(
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<QueryParametersModel>(request);
        var result = await semesterService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<SemesterResponse>>(result.Items);
        var page = ToPageResponse<SemesterResponse, SemesterModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Semesters retrieved successfully."));
    }

    /// <summary>
    /// Gets a semester by identifier with related course and subject data.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetSemester(
        int id,
        CancellationToken cancellationToken)
    {
        var semester = await semesterService.GetByIdAsync(id, cancellationToken);
        var response = mapper.Map<SemesterResponse>(semester);

        return Ok(new ApiResponse<SemesterResponse>(response, "Semester retrieved successfully."));
    }

    /// <summary>
    /// Creates a semester.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> CreateSemester(
        [FromBody] CreateSemesterRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<SemesterModel>(request);
        var created = await semesterService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<SemesterResponse>(created);

        return CreatedAtAction(
            nameof(GetSemester),
            new { id = response.SemesterId },
            new ApiResponse<SemesterResponse>(response, "Semester created successfully."));
    }

    /// <summary>
    /// Updates a semester.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> UpdateSemester(
        int id,
        [FromBody] UpdateSemesterRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<SemesterModel>(request);
        var updated = await semesterService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<SemesterResponse>(updated);

        return Ok(new ApiResponse<SemesterResponse>(response, "Semester updated successfully."));
    }

    /// <summary>
    /// Deletes a semester.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteSemester(
        int id,
        CancellationToken cancellationToken)
    {
        await semesterService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Semester deleted successfully."));
    }
}
