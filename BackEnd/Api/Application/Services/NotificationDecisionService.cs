namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;

public class NotificationDecisionService : INotificationDecisionService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;
    private readonly IDockRepository _dockRepository;
    private readonly ILogger<NotificationDecisionService> _logger;

    public NotificationDecisionService(IVesselVisitNotificationRepository notificationRepository, IDockRepository dockRepository, ILogger<NotificationDecisionService> logger)
    {
        _notificationRepository = notificationRepository;
        _dockRepository = dockRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationDecisionDto>> GetNotificationDecisions(string vesselVisitNotificationId )
    {
        bool exists = await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vesselVisitNotificationId) != null;
        if (!exists)
            throw new EntityNotFoundException($"Vessel Visit Notification with id {vesselVisitNotificationId} was not found.");
            
        var decisions = await _notificationRepository.GetNotificationDecisionsAsync(vesselVisitNotificationId);
        
        AppLogEvents.LogRetrieve(_logger, "notification decisions", decisions.Count());
        return decisions.Select(n => n.ToDTO()).ToList();
    }

    public async Task<NotificationDecisionDto> Add(CreateNotificationDecisionDto notificationDecisionDto, string vesselVisitNotificationId, string officerEmail)
    {
        VesselVisitNotification? notification = await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vesselVisitNotificationId);

        if (notification == null)
            throw new EntityNotFoundException("Vessel Visit Notification not found.");

        NotificationDecision notificationDecision;

        if (notificationDecisionDto.Status == 1)
        {
            if (notificationDecisionDto.AssignedDockCode == null)
                throw new ArgumentException("AssignedDock must be provided for accepted decisions.", nameof(notificationDecisionDto.AssignedDockCode));

            Dock? assignedDock = await _dockRepository.GetDockByCodeAsync(notificationDecisionDto.AssignedDockCode);
            if (assignedDock == null)
                throw new EntityNotFoundException("Assigned Dock not found.");

            notificationDecision = NotificationDecisionFactory.CreateAccepted(officerEmail, notificationDecisionDto.Reason, assignedDock, notificationDecisionDto.DecisionDate);
        }
        else if (notificationDecisionDto.Status == 2)
            notificationDecision = NotificationDecisionFactory.CreateRejected(officerEmail, notificationDecisionDto.Reason ?? "No reason provided",
             notificationDecisionDto.IsFinal, notificationDecisionDto.DecisionDate);
        else
            throw new ArgumentException("Invalid status value.", nameof(notificationDecisionDto.Status));

        notification.AddDecision(notificationDecision);

        var updatedNotification = await _notificationRepository.UpdateAsync(notification);
        var createdDecision = updatedNotification.NotificationDecisions.Last();

        AppLogEvents.LogCreate(_logger, "notification decision", createdDecision.Id);
        return createdDecision.ToDTO();
    }
}