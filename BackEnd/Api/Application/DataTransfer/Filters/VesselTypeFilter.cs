namespace Api.Application.DataTransfer.Filters;

using Api.Infrastructure.Utilities;

public class VesselTypeFilter : Pageable
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}