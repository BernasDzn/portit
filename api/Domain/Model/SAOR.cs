namespace Api.Models;

public class ShippingAgentOrganizationRepresentative
{
    private Guid _id;
    private string _name;
    private uint _citizenshipId;
    private string _email;
    private string _phone;

    public Guid Id { get => _id; }
    public string Name { get => _name; }
    public uint CitizenshipId { get => _citizenshipId; }
    public string Email { get => _email; }
    public string Phone { get => _phone; }

    public ShippingAgentOrganizationRepresentative(Guid id, string name, uint citizenshipId, string email, string phone) {
        _id = id;
        _name = name;
        _citizenshipId = citizenshipId;
        _email = email;
        _phone = phone;
    }
}