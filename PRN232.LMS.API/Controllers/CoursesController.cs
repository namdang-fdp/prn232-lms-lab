using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[Route("api/courses")]
[Produces("application/json")]
public class CoursesController(ICourseService courseService, IMapper mapper) : LmsControllerBase
{
    /// <summary>
    /// Gets a paged list of courses.
    /// </summary>
    /// <remarks>Supports search, sort, paging, field selection, and relationship expansion.</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetCourses(
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<QueryParametersModel>(request);
        var result = await courseService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<CourseResponse>>(result.Items);
        var page = ToPageResponse<CourseResponse, CourseModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Courses retrieved successfully."));
    }

    /// <summary>
    /// Gets a course by identifier with related semester, subject, and enrollment data.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourse(
        int id,
        CancellationToken cancellationToken)
    {
        var course = await courseService.GetByIdAsync(id, cancellationToken);
        var response = mapper.Map<CourseResponse>(course);

        return Ok(new ApiResponse<CourseResponse>(response, "Course retrieved successfully."));
    }

    /// <summary>
    /// Creates a course.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> CreateCourse(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<CourseModel>(request);
        var created = await courseService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<CourseResponse>(created);

        return CreatedAtAction(
            nameof(GetCourse),
            new { id = response.CourseId },
            new ApiResponse<CourseResponse>(response, "Course created successfully."));
    }

    /// <summary>
    /// Updates a course.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> UpdateCourse(
        int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<CourseModel>(request);
        var updated = await courseService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<CourseResponse>(updated);

        return Ok(new ApiResponse<CourseResponse>(response, "Course updated successfully."));
    }

    /// <summary>
    /// Deletes a course.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteCourse(
        int id,
        CancellationToken cancellationToken)
    {
        await courseService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Course deleted successfully."));
    }
}
