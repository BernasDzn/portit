using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class VesselDto
{
    public required string Name { get; set; }
    public required string ImoNumber { get; set; }
    public required VesselTypeDto Type { get; set; }
    public required ShippingAgentOrganizationDto Owner { get; set; }
    public required double Length { get; set; }
    public required double Depth { get; set; }
    public required double Draft { get; set; }

}