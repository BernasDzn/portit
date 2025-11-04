namespace Api.Application.DataTransfer.Filters;

using Api.Infrastructure.Utilities;

public class SystemUserFilter : Pageable
{
    public string? Email { get; set; }
    public int? Role { get; set; }
    public string? Sub { get; set; }
    public bool? IsActive { get; set; }
}