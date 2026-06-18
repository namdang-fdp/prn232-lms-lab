using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[Route("api/subjects")]
[Produces("application/json", "application/xml")]
public class SubjectsController(ISubjectService subjectService, IMapper mapper) : LmsControllerBase
{
    /// <summary>
    /// Gets a paged list of subjects.
    /// </summary>
    /// <remarks>Supports search, sort, paging, field selection, and relationship expansion.</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetSubjects(
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<QueryParametersBusinessModel>(request);
        var result = await subjectService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<SubjectResponse>>(result.Items);
        var page = ToPageResponse<SubjectResponse, SubjectBusinessModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Subjects retrieved successfully."));
    }

    /// <summary>
    /// Gets a subject by identifier with related course and semester data.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetSubject(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var subject = await subjectService.GetByIdAsync(id, cancellationToken);
            var response = mapper.Map<SubjectResponse>(subject);

            return Ok(new ApiResponse<SubjectResponse>(response, "Subject retrieved successfully."));
        }
        catch (ServiceException exception) when (exception.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFoundResponse(exception);
        }
    }

    /// <summary>
    /// Creates a subject.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> CreateSubject(
        [FromBody] CreateSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<SubjectBusinessModel>(request);
        var created = await subjectService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<SubjectResponse>(created);

        return CreatedAtAction(
            nameof(GetSubject),
            new { id = response.SubjectId },
            new ApiResponse<SubjectResponse>(response, "Subject created successfully."));
    }

    /// <summary>
    /// Updates a subject.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> UpdateSubject(
        int id,
        [FromBody] UpdateSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<SubjectBusinessModel>(request);
        var updated = await subjectService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<SubjectResponse>(updated);

        return Ok(new ApiResponse<SubjectResponse>(response, "Subject updated successfully."));
    }

    /// <summary>
    /// Deletes a subject.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteSubject(
        int id,
        CancellationToken cancellationToken)
    {
        await subjectService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Subject deleted successfully."));
    }
}
