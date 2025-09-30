public class ShippingAgentOrganizationDto
{
    public required string Name { get; set; }
    public required string[] AltNames { get; set; }
    public required string TaxNumber { get; set; }
    public required virtual AddressDto Address { get; set; }
    public required virtual List<RepresentativeDto> Representatives { get; set; }
}

public class RepresentativeDto
{
    public required string Name { get; set; }
    public required uint CitizenshipId { get; set; }
    public required string EmailAddress { get; set; }
    public required string Phone { get; set; }
}
