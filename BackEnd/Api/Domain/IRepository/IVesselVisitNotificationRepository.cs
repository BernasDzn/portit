using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Infrastructure.Utilities;

namespace Api.Domain.IRepository;

public interface IVesselVisitNotificationRepository : IGenericRepository<VesselVisitNotification>
{
	Task<IEnumerable<VesselVisitNotification>> GetVesselVisitNotificationsAsync();
	Task<VesselVisitNotification> GetVesselVisitNotificationByIdAsync(Guid id);
	Task<VesselVisitNotification> GetVesselVisitNotificationByVesselIMOAsync(string imoNumber);
	Task<IEnumerable<NotificationDecision>> GetNotificationDecisionsAsync(Guid notificationId);
	Task<VesselVisitNotification> AddAsync(VesselVisitNotification VesselVisitNotification);
	Task<bool> Update(VesselVisitNotification VesselVisitNotification);
	Task<NotificationDecision> AddNotificationDecisionAsync(VesselVisitNotification notification);
}