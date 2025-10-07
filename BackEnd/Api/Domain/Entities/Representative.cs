namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


public class Representative : IDTOAble<RepresentativeDto>
{
    public Guid Id { get; private set; }
    public Designation Name { get; private set; }
    public uint CitizenshipId { get; private set; }
    public Email EmailAddress { get; private set; }
    public PhoneNumber Phone { get; private set; }

    public virtual ICollection<VesselVisitNotification> VesselVisitNotifications { get; private set; }

    // EF Core
    protected Representative() { }

    public Representative(Guid id, uint citizenshipId, Designation name, Email email, PhoneNumber phone)
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
            Name = this.Name.Value,
            CitizenshipId = this.CitizenshipId,
            EmailAddress = this.EmailAddress.Value,
            Phone = this.Phone.Value
        };
    }
}
