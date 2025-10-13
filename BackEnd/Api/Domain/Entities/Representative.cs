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
    public virtual ShippingAgentOrganization? RepresentedOrganization { get; private set; } // navigation property

    // EF Core
    protected Representative() { }

    public Representative(Guid id, uint citizenshipId, Designation name, Email emailAddress, PhoneNumber phone)
    {
        Id = id;
        CitizenshipId = citizenshipId;
        Name = name;
        EmailAddress = emailAddress;
        Phone = phone;
    }

    public void AssignToOrganization(ShippingAgentOrganization organization)
    {
        if (RepresentedOrganization != null && RepresentedOrganization.Id != organization.Id)
            throw new InvalidOperationException($"Representative {Name.Value} is already representing another organization.");

        RepresentedOrganization = organization;
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
