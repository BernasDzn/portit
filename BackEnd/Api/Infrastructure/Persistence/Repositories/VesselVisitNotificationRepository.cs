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

    public async Task<VesselVisitNotification> GetVesselVisitNotificationByNotificationIdAsync(string notificationId)
    {
        try
        {
            VesselVisitNotification? notification = await _context.VesselVisitNotifications.FirstOrDefaultAsync(n => n.NotificationId.Value == notificationId);
            return notification!;
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

    public async Task<IEnumerable<NotificationDecision>> GetNotificationDecisionsAsync(string notificationId)
    {
        try
        {

            IEnumerable<NotificationDecision> decisions = await _context.VesselVisitNotifications
                .Where(n => n.NotificationId.Value == notificationId)
                .SelectMany(n => n.NotificationDecisions)
                .ToListAsync();

            return decisions;
        }
        catch
        {
            throw;
        }
    }


    public async Task<VesselVisitNotification> AddAsync(VesselVisitNotification vesselVisitNotification)
    {
        try
        {
            _context.VesselVisitNotifications.Add(vesselVisitNotification);
            await _context.SaveChangesAsync();
            return vesselVisitNotification;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> Update(VesselVisitNotification vesselVisitNotification)
    {
        try
        {
            _context.VesselVisitNotifications.Update(vesselVisitNotification);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<NotificationDecision> AddNotificationDecisionAsync(VesselVisitNotification notification)
    {
        try
        {
            _context.VesselVisitNotifications.Update(notification);
            await _context.SaveChangesAsync();
            return notification.NotificationDecisions.Last();
        }
        catch
        {
            throw;
        }
    }
}