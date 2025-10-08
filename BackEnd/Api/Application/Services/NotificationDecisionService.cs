namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Domain.Entities;
using Api.Domain.IRepository;

public class NotificationDecisionService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;

    public NotificationDecisionService(IVesselVisitNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationDecisionDto>> GetNotificationDecisions(string vesselVisitNotificationId )
    {
        var decisions = await _notificationRepository.GetNotificationDecisionsAsync(vesselVisitNotificationId );

        return decisions.Select(n => n.ToDTO()).ToList();
    }

    public async Task<NotificationDecisionDto> Add(NotificationDecisionDto notificationDecisionDto, string vesselVisitNotificationId)
    {
        VesselVisitNotification? notification = await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vesselVisitNotificationId);

        if (notification == null)
        {
            throw new Exception("Vessel Visit Notification not found.");
        }

        NotificationDecision notificationDecision = new NotificationDecision(notificationDecisionDto.Status == 1 ? NotificationDecisionStatus.Approved : NotificationDecisionStatus.Rejected,
            notificationDecisionDto.DecisionDate, notificationDecisionDto.OfficerID, null, notificationDecisionDto.Reason);

        notification.AddDecision(notificationDecision);

        var createdDecision = await _notificationRepository.AddNotificationDecisionAsync(notification);
        
        return createdDecision.ToDTO();
    }
}