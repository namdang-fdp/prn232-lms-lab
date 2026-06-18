using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class QueryRequest
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public string? Fields { get; set; }
    public string? Expand { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int Size { get; set; } = 10;
}
