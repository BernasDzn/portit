using Api.Domain.Model;

public class DockDto
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required PhysicalCharacteristicsDto PhysicalCharacteristics { get; set; }
    public required List<VesselTypeDto> SupportedVesselTypes { get; set; }
}