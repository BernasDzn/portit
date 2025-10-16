using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class VesselTypeDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required uint MaxNumberOfRows { get; set; }
    public required uint MaxNumberOfBays { get; set; }
    public required uint MaxNumberOfTiers { get; set; }
    public required PhysicalCharacteristics PhysicalCharacteristics { get; set; }
}