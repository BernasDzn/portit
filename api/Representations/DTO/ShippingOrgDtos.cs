public class ShippingAgentOrganizationDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string[] AltNames { get; set; }
    public required string TaxNumber { get; set; }
    public required AddressDto Address { get; set; }
    public required List<RepresentativeDto> Representatives { get; set; }
}

public class RepresentativeDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required uint CitizenshipId { get; set; }
    public required string EmailAddress { get; set; }
    public required string Phone { get; set; }
}
