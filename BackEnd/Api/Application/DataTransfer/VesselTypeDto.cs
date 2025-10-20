using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class VesselTypeDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required uint MaxNumberOfRows { get; set; }
    public required uint MaxNumberOfBays { get; set; }
    public required uint MaxNumberOfTiers { get; set; }
    public uint Capacity => MaxNumberOfRows * MaxNumberOfBays * MaxNumberOfTiers;
    public required double Length { get; set; }
    public required double Depth { get; set; }
    public required double Draft { get; set; }
}