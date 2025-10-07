namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
public class VesselVisitNotificationRepository : GenericRepository<VesselVisitNotification>, IVesselVisitNotificationRepository
{
    private new readonly ApiContext _context;

    public VesselVisitNotificationRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VesselVisitNotification>> GetVesselVisitNotificationsAsync()
    {
        try
        {
            IEnumerable<VesselVisitNotification> notifications = await _context.VesselVisitNotifications.ToListAsync();
            return notifications;
        }
        catch
        {
            throw;
        }
    }

    public async Task<VesselVisitNotification> GetVesselVisitNotificationByVesselIMOAsync(string imoNumber)
    {
        try
        {
            VesselVisitNotification? notification = await _context.VesselVisitNotifications.FirstOrDefaultAsync(n => n.Vessel.ImoIdentifier.Value == imoNumber);
            return notification!;
        }
        catch
        {
            throw;
        }
    }

    public new async Task<VesselVisitNotification> Add(VesselVisitNotification vesselVisitNotification)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Update(VesselVisitNotification vesselVisitNotification)
    {
        throw new NotImplementedException();
    }
}