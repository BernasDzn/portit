using Api.Domain.Entities;

namespace Api.Application.DataTransfer;

public class NotificationDecisionDto
{
    public required int Status { get; set; }
    public string? Reason { get; set; }
    public required DateTime DecisionDate { get; set; }
    public string OfficerEmail { get; set; }
    public DockDto? AssignedDock { get; set; }
    public required bool IsFinal { get; set; }
}

public class CreateNotificationDecisionDto
{
    public required int Status { get; set; }
    public string? Reason { get; set; }
    public required DateTime DecisionDate { get; set; }
    public string? OfficerEmail { get; set; }
    public string? AssignedDockCode { get; set; }
    public required bool IsFinal { get; set; }
}