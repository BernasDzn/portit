using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

namespace Api.Domain.Entities;


public enum NotificationDecisionStatus
{
    In_Progress = 0,
    Approved = 1,
    Rejected = 2
}

public class NotificationDecision : IDTOAble<NotificationDecisionDto>
{
    public Guid Id { get; private set; }
    public NotificationDecisionStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public DateTime DecisionDate { get; private set; }
    public int? OfficerID { get; private set; }
    public virtual Dock? AssignedDock { get; private set; }
    public virtual VesselVisitNotification VesselVisitNotification { get; private set; }

    protected NotificationDecision() { }
    public NotificationDecision(NotificationDecisionStatus status, string? reason, DateTime decisionDate, int? officerID, Dock? assignedDock, VesselVisitNotification vesselVisitNotification)
    {
        Id = Guid.NewGuid();
        Status = status;
        Reason = reason;
        DecisionDate = decisionDate;
        OfficerID = officerID;
        AssignedDock = assignedDock;
        VesselVisitNotification = vesselVisitNotification;
    }

    public NotificationDecisionDto ToDTO()
    {
        return new NotificationDecisionDto
        {
            Id = Id,
            Status = (int)Status,
            Reason = Reason,
            DecisionDate = DecisionDate,
            OfficerID = OfficerID,
            AssignedDock = AssignedDock?.ToDTO(),
            VesselVisitNotification = VesselVisitNotification.ToDTO()
        };
    }

}