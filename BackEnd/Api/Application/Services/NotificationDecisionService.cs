namespace Api.Application.Services;

using Api.Application.DataTransfer;
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
}