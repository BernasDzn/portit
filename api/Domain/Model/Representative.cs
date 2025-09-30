using System.Text.Json.Serialization;
using Domain.Model.Generic;

namespace Api.Models;

public class Representative : IDTOAble<RepresentativeDto>
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public uint CitizenshipId { get; private set; }
    public string EmailAddress { get; private set; }
    public string Phone { get; private set; }

    // EF Core
    protected Representative() { }

    public Representative(Guid id, uint citizenshipId, string name, string email, string phone)
    {
        Id = id;
        CitizenshipId = citizenshipId;
        Name = name;
        EmailAddress = email;
        Phone = phone;
    }

    public RepresentativeDto ToDTO()
    {
        return new RepresentativeDto
        {
            Name = this.Name,
            CitizenshipId = this.CitizenshipId,
            EmailAddress = this.EmailAddress,
            Phone = this.Phone
        };
    }
}
