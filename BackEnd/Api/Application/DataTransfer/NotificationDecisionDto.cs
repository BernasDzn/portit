using Api.Domain.Entities;

namespace Api.Application.DataTransfer;

public class NotificationDecisionDto
{
    public Guid Id { get; set; }
    public int Status { get; set; }
    public string? Reason { get; set; }
    public DateTime DecisionDate { get; set; }
    public int? OfficerID { get; set; }
    public DockDto? AssignedDock { get; set; }
    public VesselVisitNotificationDto VesselVisitNotification { get; set; }
    
}