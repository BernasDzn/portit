using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class DockDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required double Length { get; set; }
    public required double Depth { get; set; }
    public required double Draft { get; set; }
    public required List<VesselTypeDto> SupportedVesselTypes { get; set; }
}