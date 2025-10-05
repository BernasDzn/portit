namespace Api.Application.DataTransfer.Filters;

using Api.Infrastructure.Utilities;

public class DockFilter : Pageable
{
    public string? DockName { get; set; }
    public string? Location { get; set; }
    public string? VesselTypeName { get; set; }
}