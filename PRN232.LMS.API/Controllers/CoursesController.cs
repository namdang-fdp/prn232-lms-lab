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
public class CoursesController(ICourseService courseService, IMapper mapper) : LmsControllerBase
{
    [HttpGet]
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourse(
        int id,
        CancellationToken cancellationToken)
    {
        var course = await courseService.GetByIdAsync(id, cancellationToken);
        var response = mapper.Map<CourseResponse>(course);

        return Ok(new ApiResponse<CourseResponse>(response, "Course retrieved successfully."));
    }

    [HttpPost]
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

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteCourse(
        int id,
        CancellationToken cancellationToken)
    {
        await courseService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Course deleted successfully."));
    }
}
