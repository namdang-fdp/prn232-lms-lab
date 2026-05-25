using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[Route("api/students")]
public class StudentsController(IStudentService studentService, IMapper mapper) : LmsControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResponse<object>>>> GetStudents(
        [FromQuery] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<QueryParametersModel>(request);
        var result = await studentService.GetAsync(query, cancellationToken);
        var responses = mapper.Map<IReadOnlyList<StudentResponse>>(result.Items);
        var page = ToPageResponse<StudentResponse, StudentModel>(result, responses, request.Fields);

        return Ok(new ApiResponse<PageResponse<object>>(page, "Students retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> GetStudent(
        int id,
        CancellationToken cancellationToken)
    {
        var student = await studentService.GetByIdAsync(id, cancellationToken);
        var response = mapper.Map<StudentResponse>(student);

        return Ok(new ApiResponse<StudentResponse>(response, "Student retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> CreateStudent(
        [FromBody] CreateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<StudentModel>(request);
        var created = await studentService.CreateAsync(model, cancellationToken);
        var response = mapper.Map<StudentResponse>(created);

        return CreatedAtAction(
            nameof(GetStudent),
            new { id = response.StudentId },
            new ApiResponse<StudentResponse>(response, "Student created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> UpdateStudent(
        int id,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var model = mapper.Map<StudentModel>(request);
        var updated = await studentService.UpdateAsync(id, model, cancellationToken);
        var response = mapper.Map<StudentResponse>(updated);

        return Ok(new ApiResponse<StudentResponse>(response, "Student updated successfully."));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteStudent(
        int id,
        CancellationToken cancellationToken)
    {
        await studentService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponse<object?>(null, "Student deleted successfully."));
    }
}
