using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Entities;

public enum NotificationDecisionStatus
{
    Approved = 1,
    Rejected = 2
}

public class NotificationDecision : IDTOAble<NotificationDecisionDto>
{
    public Guid Id { get; private set; }
    public NotificationDecisionStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public DateTime DecisionDate { get; private set; }
    public string OfficerEmail { get; private set; }
    public bool isFinal = false;
    public virtual Dock? AssignedDock { get; private set; }

    protected NotificationDecision() { }
    public NotificationDecision(NotificationDecisionStatus status, DateTime decisionDate, string officerEmail, Dock? assignedDock = null, string? reason = null)
    {
        Status = status;
        Reason = reason;
        DecisionDate = decisionDate;
        OfficerEmail = officerEmail;
        AssignedDock = assignedDock;
    }

    public void MarkAsFinal()
    {
        isFinal = true;
    }

    public NotificationDecisionDto ToDTO()
    {
        return new NotificationDecisionDto
        {
            Status = (int)Status,
            Reason = Reason,
            DecisionDate = DecisionDate,
            OfficerEmail = OfficerEmail,
            AssignedDock = AssignedDock?.ToDTO(),
            IsFinal = isFinal
        };
    }

}