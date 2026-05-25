using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[Route("api/subjects")]
public class SubjectsController(ISubjectService subjectService, IMapper mapper) : LmsControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetSubjects(
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<QueryParametersModel>(request);
        var result = await subjectService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<SubjectResponse>>(result.Items);
        var page = ToPageResponse<SubjectResponse, SubjectModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Subjects retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetSubject(
        int id,
        CancellationToken cancellationToken)
    {
        var subject = await subjectService.GetByIdAsync(id, cancellationToken);
        var response = mapper.Map<SubjectResponse>(subject);

        return Ok(new ApiResponse<SubjectResponse>(response, "Subject retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> CreateSubject(
        [FromBody] CreateSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<SubjectModel>(request);
        var created = await subjectService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<SubjectResponse>(created);

        return CreatedAtAction(
            nameof(GetSubject),
            new { id = response.SubjectId },
            new ApiResponse<SubjectResponse>(response, "Subject created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> UpdateSubject(
        int id,
        [FromBody] UpdateSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<SubjectModel>(request);
        var updated = await subjectService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<SubjectResponse>(updated);

        return Ok(new ApiResponse<SubjectResponse>(response, "Subject updated successfully."));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteSubject(
        int id,
        CancellationToken cancellationToken)
    {
        await subjectService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Subject deleted successfully."));
    }
}
