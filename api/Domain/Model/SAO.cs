namespace Api.Models;

using global::Domain.Model.Generic;

public class ShippingAgentOrganization
{
    private Guid _id;
    private string _legalName;
    private List<string> _altNames;
    private Address _address;
    private string _taxId;
    private List<Representative> _representatives;

    public Guid Id { get => _id; set => _id = value; }
    public string LegalName { get => _legalName; set => _legalName = value; }
    public List<string> AltNames { get => _altNames; set => _altNames = value; }
    public Address MainAddress { get => _address; set => _address = value; }
    public string TaxId { get => _taxId; set => _taxId = value; }
    public List<Representative> Representatives { get => _representatives; set => _representatives = value; }

    //EF Core
    private ShippingAgentOrganization() { }

    public ShippingAgentOrganization(Guid id, string legalName, List<string> altNames, Address address, string taxId, List<Representative> representatives)
    {
        _id = id;
        _legalName = legalName;
        _altNames = altNames;
        _address = address;
        _taxId = taxId;
        _representatives = representatives;
    }
}