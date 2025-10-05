namespace Api.Application.DataTransfer.Filters;

using Api.Infrastructure.Utilities;

public class VesselFilter : Pageable
{
    public string? Name { get; set; }
    public string? ImoNumber { get; set; }
    public string? TaxNumber { get; set; }
}