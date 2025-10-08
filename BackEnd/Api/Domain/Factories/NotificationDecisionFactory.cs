namespace Api.Domain.Entities;

public class NotificationDecisionFactory
{
    public static NotificationDecision CreateAccepted(string? reason = null, Dock? assignedDock = null, DateTime decisionDate = default)
    {
        if (decisionDate == default)
            decisionDate = DateTime.UtcNow;

        NotificationDecision decision = new NotificationDecision(NotificationDecisionStatus.Approved, decisionDate, assignedDock: assignedDock, reason: reason);
        decision.MarkAsFinal();
        return decision;
    }

    public static NotificationDecision CreateRejected(string reason, bool isPermanent, DateTime decisionDate = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason must be provided for rejection.", nameof(reason));

        if (decisionDate == default)
            decisionDate = DateTime.UtcNow;

        NotificationDecision decision = new NotificationDecision(NotificationDecisionStatus.Rejected, decisionDate, reason: reason);
        if (isPermanent)
            decision.MarkAsFinal();
        
        return decision;
    }
}