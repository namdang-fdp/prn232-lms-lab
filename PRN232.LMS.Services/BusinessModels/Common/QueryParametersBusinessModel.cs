namespace PRN232.LMS.Services.BusinessModels.Common;

public class QueryParametersBusinessModel
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public string? Fields { get; set; }
    public string? Expand { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
