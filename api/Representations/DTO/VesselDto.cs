using Api.Domain.Model;

public class VesselDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string ImoNumber { get; set; }
    public required VesselType Type { get; set; } 
    public required uint OwnerCitizenshipId { get; set; }

}