using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/students")]
[Produces("application/json", "application/xml")]
public class StudentsController(IStudentService studentService, IMapper mapper) : LmsControllerBase
{
    /// <summary>
    /// Gets a paged list of students.
    /// </summary>
    /// <remarks>Supports search, sort, paging, field selection, and relationship expansion.</remarks>
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

    /// <summary>
    /// Gets a student by identifier with related enrollment and course data.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetStudentByIdV1")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> GetStudent(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var student = await studentService.GetByIdAsync(id, cancellationToken);
            var response = mapper.Map<StudentResponse>(student);

            return Ok(new ApiResponse<StudentResponse>(response, "Student retrieved successfully."));
        }
        catch (ServiceException exception) when (exception.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFoundResponse(exception);
        }
    }

    /// <summary>
    /// Creates a student.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> CreateStudent(
        [FromBody] CreateStudentRequest request,
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        CancellationToken cancellationToken)
    {
        _ = requestId;

        var model = mapper.Map<StudentBusinessModel>(request);
        var created = await studentService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<StudentResponse>(created);

        return CreatedAtRoute(
            "GetStudentByIdV1",
            new { version = "1", id = response.StudentId },
            new ApiResponse<StudentResponse>(response, "Student created successfully."));
    }

    /// <summary>
    /// Updates a student.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> UpdateStudent(
        [FromRoute] int id,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<StudentBusinessModel>(request);
        var updated = await studentService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<StudentResponse>(updated);

        return Ok(new ApiResponse<StudentResponse>(response, "Student updated successfully."));
    }

    /// <summary>
    /// Deletes a student.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteStudent(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await studentService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Student deleted successfully."));
    }
}
