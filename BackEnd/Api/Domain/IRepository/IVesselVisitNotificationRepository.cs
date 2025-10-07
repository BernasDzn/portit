using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Infrastructure.Utilities;

namespace Api.Domain.IRepository;

public interface IVesselVisitNotificationRepository : IGenericRepository<VesselVisitNotification>
{
    Task<IEnumerable<VesselVisitNotification>> GetVesselVisitNotificationsAsync();
	Task<VesselVisitNotification> GetVesselVisitNotificationByVesselIMOAsync(string imoNumber);
	new Task<VesselVisitNotification> Add(VesselVisitNotification VesselVisitNotification);
	Task<bool> Update(VesselVisitNotification VesselVisitNotification);
}