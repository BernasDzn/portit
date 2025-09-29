namespace Api.Models;

using global::Domain.Model.Generic;

public class ShippingAgentOrganization : IDTOAble<ShippingAgentOrganizationDto>
{
    public Guid Id { get; private set; }
    public string LegalName { get; private set; }
    public List<string> AltNames { get; private set; }
    public virtual Address MainAddress { get; private set; }
    public string TaxId { get; private set; }
    public virtual ICollection<Representative> Representatives { get; private set; }

    // EF Core needs a parameterless constructor
    public ShippingAgentOrganization() { }

    public ShippingAgentOrganization(Guid id, string legalName, List<string> altNames, Address address, string taxId, List<Representative> representatives)
    {
        if (representatives == null || representatives.Count == 0)
            throw new ArgumentException("An SAO must have at least one representative.");

        Id = id;
        LegalName = legalName;
        AltNames = altNames;
        MainAddress = address;
        TaxId = taxId;
        Representatives = representatives;
    }

    public ShippingAgentOrganizationDto ToDTO()
    {
        return new ShippingAgentOrganizationDto
        {
            Id = this.Id,
            Name = this.LegalName,
            AltNames = this.AltNames.ToArray(),
            Address = this.MainAddress.ToDTO(),
            TaxNumber = this.TaxId,
            Representatives = this.Representatives.Select(r => r.ToDTO()).ToList()
        };
    }
}
