namespace Api.Application.DataTransfer;

using Api.Domain.Entities;

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

public class CreateDockRelationDto
{
    public required string DockCode { get; set; }
    public required uint? Distance { get; set; }
    public bool IsServingDock { get; set; }
}

public class CreateStorageAreaDto
{
    public required string NameCode { get; set; }
    public required string Location { get; set; }
    public required StorageAreaType Type { get; set; }
    public required uint Capacity { get; set; }
    public required uint CurrentOccupancy { get; set; }
    public required HashSet<CreateDockRelationDto>? DockServices { get; set; }
}