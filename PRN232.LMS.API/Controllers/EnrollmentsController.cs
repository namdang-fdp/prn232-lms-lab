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
[Route("api/v{version:apiVersion}/enrollments")]
[Produces("application/json", "application/xml")]
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
        var query = mapper.Map<QueryParametersBusinessModel>(request);
        var result = await enrollmentService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<EnrollmentResponse>>(result.Items);
        var page = ToPageResponse<EnrollmentResponse, EnrollmentBusinessModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Enrollments retrieved successfully."));
    }

    /// <summary>
    /// Gets an enrollment by identifier with related student, course, subject, and semester data.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetEnrollmentByIdV1")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> GetEnrollment(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await enrollmentService.GetByIdAsync(id, cancellationToken);
            var response = mapper.Map<EnrollmentResponse>(enrollment);

            return Ok(new ApiResponse<EnrollmentResponse>(response, "Enrollment retrieved successfully."));
        }
        catch (ServiceException exception) when (exception.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFoundResponse(exception);
        }
    }

    /// <summary>
    /// Creates an enrollment.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> CreateEnrollment(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<EnrollmentBusinessModel>(request);
        var created = await enrollmentService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<EnrollmentResponse>(created);

        return CreatedAtRoute(
            "GetEnrollmentByIdV1",
            new { version = "1", id = response.EnrollmentId },
            new ApiResponse<EnrollmentResponse>(response, "Enrollment created successfully."));
    }

    /// <summary>
    /// Updates an enrollment.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> UpdateEnrollment(
        [FromRoute] int id,
        [FromBody] UpdateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<EnrollmentBusinessModel>(request);
        var updated = await enrollmentService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<EnrollmentResponse>(updated);

        return Ok(new ApiResponse<EnrollmentResponse>(response, "Enrollment updated successfully."));
    }

    /// <summary>
    /// Deletes an enrollment.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteEnrollment(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await enrollmentService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Enrollment deleted successfully."));
    }
}
