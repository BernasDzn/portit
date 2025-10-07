using Api.Application.DataTransfer;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

namespace Api.Domain.Entities;

public class VesselVisitNotification : IDTOAble<VesselVisitNotificationDto>
{
    public Guid Id { get; private set; }
    public DateTime ExpectedArrival { get; private set; }
    public DateTime ExpectedDeparture { get; private set; }
    public bool IsCargoHazardous { get; private set; }
    public string? SpecialRequirements { get; private set; }
    public virtual Crew? CrewDetails { get; private set; }

    //TODO: Cargo manifest

    public virtual Vessel Vessel { get; private set; }
    public virtual Representative Representative { get; private set; }
    public virtual ICollection<NotificationDecision> NotificationDecision { get; private set; }

    protected VesselVisitNotification() { }

    public VesselVisitNotification(DateTime expectedArrival, DateTime expectedDeparture, bool isCargoHazardous, Vessel vessel, Representative representative,
     string? specialRequirements = null, Crew? crewDetails = null)
    {
        Id = Guid.NewGuid();
        ExpectedArrival = expectedArrival;
        ExpectedDeparture = expectedDeparture;
        IsCargoHazardous = isCargoHazardous;
        SpecialRequirements = specialRequirements;
        CrewDetails = crewDetails;
        Vessel = vessel;
        Representative = representative;
    }

    public VesselVisitNotificationDto ToDTO()
    {
        return new VesselVisitNotificationDto
        {
            ExpectedArrival = ExpectedArrival,
            ExpectedDeparture = ExpectedDeparture,
            IsCargoHazardous = IsCargoHazardous,
            SpecialRequirements = SpecialRequirements,
            CrewDetails = CrewDetails?.ToDTO(),
            Vessel = Vessel.ToDTO(),
            Representative = Representative.ToDTO()
        };
    }
}