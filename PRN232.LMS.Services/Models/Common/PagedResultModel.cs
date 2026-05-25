namespace PRN232.LMS.Services.Models.Common;

public class PagedResultModel<TModel>
{
    public IReadOnlyList<TModel> Items { get; set; } = Array.Empty<TModel>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
}
