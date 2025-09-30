using Api.Domain.Model;
using Api.Models;

public class VesselDto
{
    public required string Name { get; set; }
    public required string ImoNumber { get; set; }
    public required VesselType Type { get; set; }
    public required ShippingAgentOrganizationDto Owner { get; set; }

}