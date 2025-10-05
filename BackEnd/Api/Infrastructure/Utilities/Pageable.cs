namespace Api.Infrastructure.Utilities;

// Utility class to represent pagination parameters
public class Pageable
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// Page class to hold paginated results
public class Page<T>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public List<T> Items { get; set; } = new List<T>();

    internal Page<U> Map<U>(Func<T, U> value)
    {
        return new Page<U>
        {
            Items = this.Items.Select(value).ToList(),
            PageNumber = this.PageNumber,
            PageSize = this.PageSize
        };
    }

    public static Page<T> Empty() => new Page<T> { Items = new List<T>() };
    public static Page<T> Of(List<T> items, Pageable pageable) => new Page<T>
    {
        Items = items,
        PageNumber = pageable.PageNumber,
        PageSize = pageable.PageSize
    };
}