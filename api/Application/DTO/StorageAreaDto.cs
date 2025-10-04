using Api.Domain.Model;
using Api.Models;

public class DockRelationDto
{
    public required DockDto Dock { get; set; }
    public required uint? Distance { get; set; }
    public bool IsServingDock { get; set; }
}

public class StorageAreaDto
{
    public required string NameCode { get; set; }
    public required string Location { get; set; }
    public required StorageAreaType Type { get; set; }
    public required uint Capacity { get; set; }
    public required uint CurrentOccupancy { get; set; }
    public required HashSet<DockRelationDto>? DockServices { get; set; }
}