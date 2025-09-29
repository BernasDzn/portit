namespace Api.Models;

using global::Domain.Model.Generic;

public class ShippingAgentOrganization
{
    private Guid _id;
    private string _legalName;
    private List<string> _altNames;
    private Address _address;
    private string _taxId;
    private List<ShippingAgentOrganizationRepresentative> _representatives;

    public Guid Id { get => _id; }
    public string LegalName { get => _legalName; }
    public List<string> AltNames { get => _altNames; }
    public Address MainAddress { get => _address; }
    public string TaxId { get => _taxId; }
    public List<ShippingAgentOrganizationRepresentative> Representatives { get => _representatives; }

    public ShippingAgentOrganization(Guid id, string legalName, List<string> altNames, Address address, string taxId, List<ShippingAgentOrganizationRepresentative> representatives)
    {
        _id = id;
        _legalName = legalName;
        _altNames = altNames;
        _address = address;
        _taxId = taxId;
        _representatives = representatives;
    }
}