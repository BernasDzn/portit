namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Domain.Entities;
using Api.Domain.IRepository;

public class NotificationDecisionService : INotificationDecisionService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;
    private readonly IDockRepository _dockRepository;

    public NotificationDecisionService(IVesselVisitNotificationRepository notificationRepository, IDockRepository dockRepository)
    {
        _notificationRepository = notificationRepository;
        _dockRepository = dockRepository;
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

        NotificationDecision notificationDecision;

        if (notificationDecisionDto.Status == 1)
        {
            if (notificationDecisionDto.AssignedDock == null)
                throw new ArgumentException("AssignedDock must be provided for accepted decisions.", nameof(notificationDecisionDto.AssignedDock));
            
            Dock assignedDock = await _dockRepository.GetDockByNameAsync(notificationDecisionDto.AssignedDock.Name);
            

            notificationDecision = NotificationDecisionFactory.CreateAccepted(
                reason: notificationDecisionDto.Reason,
                assignedDock: assignedDock,
                decisionDate: notificationDecisionDto.DecisionDate);
        }
        else if (notificationDecisionDto.Status == 2)
        {
            notificationDecision = NotificationDecisionFactory.CreateRejected(
                reason: notificationDecisionDto.Reason ?? "No reason provided",
                isPermanent: notificationDecisionDto.IsFinal,
                decisionDate: notificationDecisionDto.DecisionDate);
        }
        else
            throw new ArgumentException("Invalid status value.", nameof(notificationDecisionDto.Status));


        notification.AddDecision(notificationDecision);

        var updatedNotification = await _notificationRepository.UpdateAsync(notification);
        var createdDecision = updatedNotification.NotificationDecisions.Last();
        
        return createdDecision.ToDTO();
    }
}