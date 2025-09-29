using Domain.Model.Generic;

namespace Api.Models;

public class Representative
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public uint CitizenshipId { get; private set; }
    public string EmailAddress { get; private set; }
    public string Phone { get; private set; }

    private Representative() { } // Required for EF

    public Representative(Guid id, uint citizenshipId, string name, string email, string phone)
    {
        Id = id;
        CitizenshipId = citizenshipId;
        Name = name;
        EmailAddress = email;
        Phone = phone;
    }
}
