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
public class SemestersController(ISemesterService semesterService, IMapper mapper) : LmsControllerBase
{
    [HttpGet]
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetSemester(
        int id,
        CancellationToken cancellationToken)
    {
        var semester = await semesterService.GetByIdAsync(id, cancellationToken);
        var response = mapper.Map<SemesterResponse>(semester);

        return Ok(new ApiResponse<SemesterResponse>(response, "Semester retrieved successfully."));
    }

    [HttpPost]
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

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteSemester(
        int id,
        CancellationToken cancellationToken)
    {
        await semesterService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Semester deleted successfully."));
    }
}
