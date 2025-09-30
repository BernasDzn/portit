public class DockDto
{
    public Guid Id { get; set; }
    public required string Designation { get; set; }
    public required string Location { get; set; }
    public required uint Length { get; set; }
    public required uint Depth { get; set; }
    public required uint MaxDraft { get; set; }
    public required List<VesselTypeDto> SupportedVesselTypes { get; set; }
}