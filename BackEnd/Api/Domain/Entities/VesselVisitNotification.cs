using Api.Application.DataTransfer;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

namespace Api.Domain.Entities;

public class VesselVisitNotification : IDTOAble<VesselVisitNotificationDto>
{
    public Guid Id { get; private set; }
    public DateTime ExpectedArrival { get; private set; }
    public DateTime ExpectedDeparture { get; private set; }
    public bool isCargoHazardous { get; private set; }
    public string? specialRequirements { get; private set; }
    public Crew? CrewDetails { get; private set; }

    //TODO: Cargo manifest

    public virtual Vessel Vessel { get; private set; }
    public virtual ICollection<NotificationDecision>? NotificationDecision { get; private set; }

    protected VesselVisitNotification() { }

    public VesselVisitNotification(DateTime expectedArrival, DateTime expectedDeparture, bool isCargoHazardous, Vessel vessel)
    {
        Id = Guid.NewGuid();
        ExpectedArrival = expectedArrival;
        ExpectedDeparture = expectedDeparture;
        this.isCargoHazardous = isCargoHazardous;
        Vessel = vessel;
    }

    public VesselVisitNotificationDto ToDTO()
    {
        return new VesselVisitNotificationDto
        {
            Id = Id,
            ExpectedArrival = ExpectedArrival,
            ExpectedDeparture = ExpectedDeparture,
            isCargoHazardous = isCargoHazardous,
            Vessel = Vessel.ToDTO(),
            NotificationDecision = NotificationDecision?.Select(nd => nd.ToDTO()).ToList()
        };
    }
}