using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using PRN232.LMS.API.Models.Responses;

namespace PRN232.LMS.API.Common.Response;

[KnownType(typeof(CourseResponse))]
[KnownType(typeof(CourseSummaryResponse))]
[KnownType(typeof(Dictionary<string, object>))]
[KnownType(typeof(EnrollmentResponse))]
[KnownType(typeof(EnrollmentSummaryResponse))]
[KnownType(typeof(SemesterResponse))]
[KnownType(typeof(SemesterSummaryResponse))]
[KnownType(typeof(StudentResponse))]
[KnownType(typeof(StudentSummaryResponse))]
[KnownType(typeof(SubjectResponse))]
[KnownType(typeof(SubjectSummaryResponse))]
public class PageResponse<T>
{
    public IEnumerable<T> Content { get; set; } = new List<T>();

    [JsonPropertyName("items")]
    public IEnumerable<T> Items
    {
        get => Content;
        set => Content = value;
    }

    public PaginationMetadata Pagination { get; set; } = new();
}

public class PaginationMetadata
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}
