using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Common.Response;

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
