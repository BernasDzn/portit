using Api.Domain.Model;

public class DockDto
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required uint Length { get; set; }
    public required uint Depth { get; set; }
    public required uint MaxDraft { get; set; }
    public required List<VesselType> SupportedVesselTypes { get; set; }
}