namespace PRN232.LMS.Services.BusinessModels.Common;

public class PagedResultBusinessModel<TBusinessModel>
{
    public IReadOnlyList<TBusinessModel> Items { get; set; } = Array.Empty<TBusinessModel>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
}
