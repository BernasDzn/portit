using System.ComponentModel.DataAnnotations.Schema;
using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
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
    public VesselVisitNotificationId NotificationId { get; private set; }
    public DateTime ExpectedArrival { get; private set; }
    public DateTime ExpectedDeparture { get; private set; }
    public bool IsCargoHazardous { get; private set; }
    public string? SpecialRequirements { get; private set; }
    [NotMapped]
    public virtual Crew? CrewDetails { get; private set; }
    public virtual ICollection<CargoTransport>? LoadCargoManifest { get; private set; }
    public virtual ICollection<CargoTransport>? UnloadCargoManifest { get; private set; }

    public virtual Vessel Vessel { get; private set; }
    public virtual Representative Submitter { get; private set; } 
    public NotificationStatus Status { get; private set; } = NotificationStatus.InProgress;
    public virtual ICollection<NotificationDecision> NotificationDecisions { get; private set; } = new LinkedList<NotificationDecision>();

    protected VesselVisitNotification() { }

    public VesselVisitNotification(
        VesselVisitNotificationId notificationId,
        DateTime expectedArrival,
        DateTime expectedDeparture,
        bool isCargoHazardous,
        Vessel vessel,
        Representative submitter,
        string? specialRequirements = null,
        Crew? crewDetails = null,
        ICollection<CargoTransport>? loadCargoManifest = null,
        ICollection<CargoTransport>? unloadCargoManifest = null
    )
    {
        Id = Guid.NewGuid();
        NotificationId = notificationId ?? throw new ArgumentNullException(nameof(notificationId));
        ExpectedArrival = expectedArrival;
        ExpectedDeparture = expectedDeparture;
        IsCargoHazardous = isCargoHazardous;
        SpecialRequirements = specialRequirements;
        CrewDetails = crewDetails;
        LoadCargoManifest = loadCargoManifest;
        UnloadCargoManifest = unloadCargoManifest;
        Vessel = vessel;

        if (!vessel.Owner.IsRepresentedBy(submitter))
            throw new InvalidRepresentativeException("The provided representative does not represent the vessel owner.");

        Submitter = submitter;
    }

    public void Update(
        DateTime expectedArrival,
        DateTime expectedDeparture,
        bool isCargoHazardous,
        string? specialRequirements = null,
        Crew? crewDetails = null,
        ICollection<CargoTransport>? loadCargoManifest = null,
        ICollection<CargoTransport>? unloadCargoManifest = null
    )
    {
        if (Status != NotificationStatus.InProgress)
            throw new InvalidOperationException("Only notifications in progress can be updated.");

        ExpectedArrival = expectedArrival;
        ExpectedDeparture = expectedDeparture;
        IsCargoHazardous = isCargoHazardous;
        SpecialRequirements = specialRequirements;
        CrewDetails = crewDetails;
        LoadCargoManifest = loadCargoManifest;
        UnloadCargoManifest = unloadCargoManifest;
	}

    public void Submit()
    {
        if (Status != NotificationStatus.InProgress)
            throw new InvalidOperationException("Only notifications in progress can be submitted.");

        Status = NotificationStatus.ApprovalPending;
    }

    private void Close()
    {
        Status = NotificationStatus.Decided;
    }

    public void AddDecision(NotificationDecision decision)
    {
        if (decision == null) { throw new ArgumentNullException(nameof(decision)); }
        if (Status != NotificationStatus.ApprovalPending) throw new InvalidOperationException("Decisions can only be added to notifications pending approval.");

        // Check if decision is from an older time than last
        NotificationDecision? latestDecision = GetLatestDecision();
        if (latestDecision != null && decision.DecisionDate < latestDecision.DecisionDate)
            throw new OutdatedDecisionException("A newer decision has already been made.");

        NotificationDecisions.Add(decision);

        // Update current status to reflect decision
        if (decision.isFinal || decision.Status == NotificationDecisionStatus.Approved) Close();
        // If rejected but not final, revert to in-progress for modifications
        else if (decision.Status == NotificationDecisionStatus.Rejected) Status = NotificationStatus.InProgress;
    }

    private NotificationDecision? GetLatestDecision() {
        return NotificationDecisions.OrderByDescending(d => d.DecisionDate).FirstOrDefault();
    }

    public override string ToString()
    {
        return $"VesselVisitNotification [Guid={Id}, NotificationId={NotificationId}, ExpectedArrival={ExpectedArrival}, ExpectedDeparture={ExpectedDeparture}, IsCargoHazardous={IsCargoHazardous}, SpecialRequirements={SpecialRequirements}, CrewDetails=({CrewDetails}), LoadCargoManifest=({LoadCargoManifest}), UnloadCargoManifest=({UnloadCargoManifest}), Vessel=({Vessel}), Submitter=({Submitter}), Status={Status}, NotificationDecisions=[{string.Join(", ", NotificationDecisions)}]]";
    }

    public VesselVisitNotificationDto ToDTO()
    {
        return new VesselVisitNotificationDto
        {
            NotificationId = NotificationId.ToString(),
            ExpectedArrival = ExpectedArrival,
            ExpectedDeparture = ExpectedDeparture,
            IsCargoHazardous = IsCargoHazardous,
            SpecialRequirements = SpecialRequirements,
            CrewDetails = CrewDetails,
            LoadCargoManifest = LoadCargoManifest?.Select(ct => ct.ToDTO()).ToList(),
            UnloadCargoManifest = UnloadCargoManifest?.Select(ct => ct.ToDTO()).ToList(),
            Vessel = Vessel.ToDTO(),
            Submitter = Submitter.ToDTO()
        };
    }
}