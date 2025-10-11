namespace Api.Infrastructure.Persistence.Repositories;

using Api.Application.Exceptions;
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

    public async Task<VesselVisitNotification> UpdateAsync(VesselVisitNotification vesselVisitNotification)
    {
        try
        {
            //Console.WriteLine(vesselVisitNotification);
            //_context.VesselVisitNotifications.Update(vesselVisitNotification);
            await _context.SaveChangesAsync();
            return vesselVisitNotification;
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException($"Failed to update vessel visit notification {vesselVisitNotification.NotificationId}: {ex.Message}");
        }
    }

}