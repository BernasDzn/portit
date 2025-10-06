namespace Api.Application.DataTransfer;

public class VesselVisitNotificationDto
{
    public required Guid Id { get; set; }
    public required DateTime ExpectedArrival { get; set; }
    public required DateTime ExpectedDeparture { get; set; }
    public required bool isCargoHazardous { get; set; }
    public required VesselDto Vessel { get; set; }
    public required List<NotificationDecisionDto>? NotificationDecision { get; set; }
}