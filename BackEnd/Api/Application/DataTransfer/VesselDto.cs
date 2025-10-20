using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class VesselDto
{
    public required string Name { get; set; }
    public required string ImoNumber { get; set; }
    public required VesselTypeDto Type { get; set; }
    public required ShippingAgentOrganizationDto Owner { get; set; }
    public required PhysicalCharacteristics PhysicalCharacteristics { get; set; }

}

public class CreateVesselDto
{
    public required string Name { get; set; }
    public required string ImoNumber { get; set; }
    public required string Type { get; set; }
    public required string Owner { get; set; }
    public required PhysicalCharacteristics PhysicalCharacteristics { get; set; }
}