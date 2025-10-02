using Api.Domain.Model;
using Api.Models;

public class DockServiceDto
{
    public required DockDto Dock { get; set; }
    public required uint Distance { get; set; }
}

public class StorageAreaDto
{
    public required string NameCode { get; set; }
    public required string Location { get; set; }
    public required StorageAreaType Type { get; set; }
    public required uint Capacity { get; set; }
    public required uint CurrentOccupancy { get; set; }
    public required HashSet<DockServiceDto> DockServices { get; set; }
}