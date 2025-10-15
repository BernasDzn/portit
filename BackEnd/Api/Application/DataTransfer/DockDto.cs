using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class DockDto
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required PhysicalCharacteristics PhysicalCharacteristics { get; set; }
    public required List<VesselTypeDto> SupportedVesselTypes { get; set; }
}