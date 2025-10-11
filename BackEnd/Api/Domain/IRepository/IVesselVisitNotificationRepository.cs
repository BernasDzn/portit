using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Infrastructure.Utilities;

namespace Api.Domain.IRepository;

public interface IVesselVisitNotificationRepository : IGenericRepository<VesselVisitNotification>
{
	Task<IEnumerable<VesselVisitNotification>> GetVesselVisitNotificationsAsync();
	Task<VesselVisitNotification> GetVesselVisitNotificationByNotificationIdAsync(string notificationId);
	Task<VesselVisitNotification> GetVesselVisitNotificationByVesselIMOAsync(string imoNumber);
	Task<IEnumerable<NotificationDecision>> GetNotificationDecisionsAsync(string notificationId);
	Task<VesselVisitNotification> AddAsync(VesselVisitNotification VesselVisitNotification);
	Task<VesselVisitNotification> UpdateAsync(VesselVisitNotification VesselVisitNotification);
} 