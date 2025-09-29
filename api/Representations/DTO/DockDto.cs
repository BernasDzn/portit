public class DockDto
{
    public Guid Id { get; set; }
    public required string Designation { get; set; }
    public required string Location { get; set; }
    public required int Length { get; set; }
    public required int Depth { get; set; }
    public required int MaxDraft { get; set; }
    public required List<VesselTypeDto> SupportedVesselTypes { get; set; }
}