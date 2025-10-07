namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Domain.Entities;
using Api.Domain.IRepository;

public class VesselVisitNotificationService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;

    public VesselVisitNotificationService(IVesselVisitNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications()
    {
        IEnumerable<VesselVisitNotification> notifications = await _notificationRepository.GetVesselVisitNotificationsAsync();
        
        return notifications.Select(n => n.ToDTO()).ToList();
    }

}