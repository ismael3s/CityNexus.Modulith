namespace CityNexus.Modulith.SharedKernel.Abstractions;

public sealed class Pagination<T>
{
    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public List<T> Items { get; set; } = [];

    public static Pagination<T> Create(int totalPages, int currentPage, int pageSize, List<T> items)
    {
        var pagination = new Pagination<T>
        {
            TotalPages = totalPages,
            CurrentPage = currentPage,
            PageSize = pageSize,
            Items = items,
        };
        return pagination;
    }
}
