using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[Route("api/enrollments")]
[Produces("application/json")]
public class EnrollmentsController(IEnrollmentService enrollmentService, IMapper mapper) : LmsControllerBase
{
    /// <summary>
    /// Gets a paged list of enrollments.
    /// </summary>
    /// <remarks>Supports search, sort, paging, field selection, and relationship expansion.</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetEnrollments(
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<QueryParametersModel>(request);
        var result = await enrollmentService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<EnrollmentResponse>>(result.Items);
        var page = ToPageResponse<EnrollmentResponse, EnrollmentModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Enrollments retrieved successfully."));
    }

    /// <summary>
    /// Gets an enrollment by identifier with related student, course, subject, and semester data.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> GetEnrollment(
        int id,
        CancellationToken cancellationToken)
    {
        var enrollment = await enrollmentService.GetByIdAsync(id, cancellationToken);
        var response = mapper.Map<EnrollmentResponse>(enrollment);

        return Ok(new ApiResponse<EnrollmentResponse>(response, "Enrollment retrieved successfully."));
    }

    /// <summary>
    /// Creates an enrollment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> CreateEnrollment(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<EnrollmentModel>(request);
        var created = await enrollmentService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<EnrollmentResponse>(created);

        return CreatedAtAction(
            nameof(GetEnrollment),
            new { id = response.EnrollmentId },
            new ApiResponse<EnrollmentResponse>(response, "Enrollment created successfully."));
    }

    /// <summary>
    /// Updates an enrollment.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> UpdateEnrollment(
        int id,
        [FromBody] UpdateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<EnrollmentModel>(request);
        var updated = await enrollmentService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<EnrollmentResponse>(updated);

        return Ok(new ApiResponse<EnrollmentResponse>(response, "Enrollment updated successfully."));
    }

    /// <summary>
    /// Deletes an enrollment.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteEnrollment(
        int id,
        CancellationToken cancellationToken)
    {
        await enrollmentService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Enrollment deleted successfully."));
    }
}
