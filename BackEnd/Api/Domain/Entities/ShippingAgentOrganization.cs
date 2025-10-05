namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


public class ShippingAgentOrganization : IDTOAble<ShippingAgentOrganizationDto>
{
    public Guid Id { get; private set; }
    public Designation LegalName { get; private set; }
    public List<Designation> AltNames { get; private set; }
    public virtual Address MainAddress { get; private set; }
    public TaxNumber TaxId { get; private set; }
    public virtual ICollection<Representative> Representatives { get; private set; }

    // EF Core
    protected ShippingAgentOrganization() { }

    public ShippingAgentOrganization(Guid id, Designation legalName, List<Designation> altNames, Address address, TaxNumber taxId, HashSet<Representative> representatives)
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
            Name = this.LegalName.Value,
            AltNames = this.AltNames.Select(n => n.Value).ToArray(),
            Address = this.MainAddress,
            TaxNumber = this.TaxId.Value,
            Representatives = this.Representatives.Select(r => r.ToDTO()).ToList()
        };
    }
}
