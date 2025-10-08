using Api.Application.DataTransfer;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

namespace Api.Domain.Entities;

public enum NotificationStatus
{
    InProgress = 0,
    ApprovalPending = 1,
    Decided = 2
}

public class VesselVisitNotification : IDTOAble<VesselVisitNotificationDto>
{
    public Guid Id { get; private set; }
    public DateTime ExpectedArrival { get; private set; }
    public DateTime ExpectedDeparture { get; private set; }
    public bool IsCargoHazardous { get; private set; }
    public string? SpecialRequirements { get; private set; }
    public virtual Crew? CrewDetails { get; private set; }

    public virtual CargoManifest? LoadCargoManifest { get; private set; }
    public virtual CargoManifest? UnloadCargoManifest { get; private set; }

    public virtual Vessel Vessel { get; private set; }
    public virtual Representative Representative { get; private set; } 
    public NotificationStatus Status { get; private set; } = NotificationStatus.InProgress;
    public virtual ICollection<NotificationDecision> NotificationDecisions { get; private set; } = new HashSet<NotificationDecision>();

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

    public void UpdateStatus(NotificationStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(NotificationStatus), newStatus)){throw new ArgumentException("Invalid status value");}

        Status = newStatus;
    }

    public void AddDecision(NotificationDecision decision)
    {
        if (decision == null) { throw new ArgumentNullException(nameof(decision)); }

        NotificationDecisions.Add(decision);
        UpdateStatus(NotificationStatus.Decided);
    }

    public VesselVisitNotificationDto ToDTO()
    {
        return new VesselVisitNotificationDto
        {
            Id = Id,
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