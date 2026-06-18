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
[Route("api/v{version:apiVersion}/courses")]
[Produces("application/json", "application/xml")]
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
        var query = mapper.Map<QueryParametersBusinessModel>(request);
        var result = await courseService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<CourseResponse>>(result.Items);
        var page = ToPageResponse<CourseResponse, CourseBusinessModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Courses retrieved successfully."));
    }

    /// <summary>
    /// Gets a course by identifier with related semester, subject, and enrollment data.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetCourseByIdV1")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourse(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var course = await courseService.GetByIdAsync(id, cancellationToken);
            var response = mapper.Map<CourseResponse>(course);

            return Ok(new ApiResponse<CourseResponse>(response, "Course retrieved successfully."));
        }
        catch (ServiceException exception) when (exception.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFoundResponse(exception);
        }
    }

    /// <summary>
    /// Gets a paged list of enrollments for a course.
    /// </summary>
    /// <remarks>Supports search, sort, paging, field selection, and relationship expansion.</remarks>
    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetEnrollmentsByCourse(
        [FromRoute] int id,
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = mapper.Map<QueryParametersBusinessModel>(request);
            var result = await courseService.GetEnrollmentsAsync(id, query, cancellationToken);
            var responses = mapper.Map<IReadOnlyList<EnrollmentResponse>>(result.Items);
            var page = ToPageResponse<EnrollmentResponse, EnrollmentBusinessModel>(result, responses, request.Fields);

            return Ok(new ApiResponse<PageResponse<object>>(page, "Course enrollments retrieved successfully."));
        }
        catch (ServiceException exception) when (exception.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFoundResponse(exception);
        }
    }

    /// <summary>
    /// Gets a paged list of students enrolled in a course.
    /// </summary>
    /// <remarks>Uses an integer route constraint for courseId and supports search, sort, paging, field selection, and relationship expansion.</remarks>
    [HttpGet("{courseId:int}/students", Name = "GetStudentsByCourseId")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetStudentsByCourse(
        [FromRoute] int courseId,
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = mapper.Map<QueryParametersBusinessModel>(request);
            var result = await courseService.GetStudentsAsync(courseId, query, cancellationToken);
            var responses = mapper.Map<IReadOnlyList<StudentResponse>>(result.Items);
            var page = ToPageResponse<StudentResponse, StudentBusinessModel>(result, responses, request.Fields);

            return Ok(new ApiResponse<PageResponse<object>>(page, "Course students retrieved successfully."));
        }
        catch (ServiceException exception) when (exception.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFoundResponse(exception);
        }
    }

    /// <summary>
    /// Creates a course.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> CreateCourse(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<CourseBusinessModel>(request);
        var created = await courseService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<CourseResponse>(created);

        return CreatedAtRoute(
            "GetCourseByIdV1",
            new { version = "1", id = response.CourseId },
            new ApiResponse<CourseResponse>(response, "Course created successfully."));
    }

    /// <summary>
    /// Updates a course.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> UpdateCourse(
        [FromRoute] int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<CourseBusinessModel>(request);
        var updated = await courseService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<CourseResponse>(updated);

        return Ok(new ApiResponse<CourseResponse>(response, "Course updated successfully."));
    }

    /// <summary>
    /// Deletes a course.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteCourse(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await courseService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Course deleted successfully."));
    }
}
