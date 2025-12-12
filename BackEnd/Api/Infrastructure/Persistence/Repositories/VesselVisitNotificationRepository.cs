namespace Api.Infrastructure.Persistence.Repositories;

using System.Collections;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Infrastructure.Utilities;
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
            throw new PersistencyFailedException("Failed to retrieve vessel visit notifications from the database.");
        }
    }

    public async Task<VesselVisitNotification?> GetVesselVisitNotificationByNotificationIdAsync(string notificationId)
    {
        try
        {
            VesselVisitNotification? notification = await _context.VesselVisitNotifications.FirstOrDefaultAsync(n => n.NotificationId.Value == notificationId);
            return notification;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve vessel visit notification by ID from the database.");
        }
    }

    public async Task<VesselVisitNotification?> GetVesselVisitNotificationByVesselIMOAsync(string imoNumber)
    {
        try
        {
            VesselVisitNotification? notification = await _context.VesselVisitNotifications.FirstOrDefaultAsync(n => n.Vessel.ImoIdentifier.Value == imoNumber);
            return notification;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve vessel visit notification by vessel IMO from the database.");
        }
    }

    public async Task<IEnumerable<NotificationDecision>> GetNotificationDecisionsAsync(string notificationId)
    {
        try
        {
            var notification = await _context.VesselVisitNotifications
                .Include(n => n.NotificationDecisions)
                .FirstOrDefaultAsync(n => n.NotificationId.Value == notificationId);

            if (notification == null)
                return Enumerable.Empty<NotificationDecision>();

            return notification.NotificationDecisions;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve notification decisions from the database.");
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
            throw new PersistencyFailedException("Failed to add vessel visit notification to the database.");
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
        catch
        {
            throw new PersistencyFailedException($"Failed to update vessel visit notification in the database.");
        }
    }
    
    public Task<Page<VesselVisitNotification>> FilterVesselVisitNotificationsPaAsync(VesselVisitNotificationFilterPa filter)
    {
        try
        {
            IQueryable<VesselVisitNotification> query = _context.VesselVisitNotifications.AsQueryable();

            if (filter.OnlyPending != null && filter.OnlyPending.Value)
            {
                query = query.Where(vvn => vvn.Status == NotificationStatus.ApprovalPending);
            }

            // Pagination
            int pageCount = (int)Math.Ceiling((double)query.Count() / filter.PageSize);
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            List<VesselVisitNotification> result = query.ToList();

            return Task.FromResult(Page<VesselVisitNotification>.Of(result, filter, pageCount));
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Task<Page<VesselVisitNotification>> FilterVesselVisitNotificationsAsync(VesselVisitNotificationFilter filter, uint userId)
    {
        try
        {
            Representative? submitter = _context.Representatives.FirstOrDefault(rep => rep.CitizenshipId == userId);
            if (submitter == null)
                throw new EntityNotFoundException($"No Representative found with Citizenship ID {userId}.");

            if (submitter.RepresentedOrganization == null)
                throw new EntityNotFoundException($"The representative with Citizenship ID {userId} does not represent any organization.");

            IQueryable<VesselVisitNotification> query = _context.VesselVisitNotifications.AsQueryable();
            ShippingAgentOrganization relatedOrg = _context.ShippingAgentOrganizations.FirstOrDefault(org => org.Representatives.Any(rep => rep.CitizenshipId == userId))
                ?? throw new EntityNotFoundException($"No Shipping Agent Organization found for Submitter Citizenship ID {userId}");

            // Apply same company rule
            query = query.Where(vvn => vvn.Submitter.RepresentedOrganization!.Id == relatedOrg.Id);

            if (filter.Status != null)
                switch (filter.Status)
                {
                    case NotificationStatusFilter.InProgress:
                        query = query.Where(vvn => vvn.Status == NotificationStatus.InProgress);
                        break;
                    case NotificationStatusFilter.ApprovalPending:
                        query = query.Where(vvn => vvn.Status == NotificationStatus.ApprovalPending);
                        break;
                    case NotificationStatusFilter.Accepted:
                    case NotificationStatusFilter.Rejected:
                        // We need to filter client side since the status is derived from GetLatestDecision()
                        query = query.Where(vvn => vvn.NotificationDecisions.Count > 0 && vvn.Status == NotificationStatus.Decided);
                        break;
                }

            if (filter.WithReason != null)
                query = filter.WithReason.Value
                    ? query.Where(vvn => vvn.NotificationDecisions.Any(nd => !string.IsNullOrEmpty(nd.Reason)))
                    : query.Where(vvn => vvn.NotificationDecisions.All(nd => string.IsNullOrEmpty(nd.Reason)));

            if (filter.WithDockAssigned != null)
                query = filter.WithDockAssigned.Value
                    ? query.Where(vvn => vvn.NotificationDecisions.Any(nd => nd.AssignedDock != null))
                    : query.Where(vvn => vvn.NotificationDecisions.All(nd => nd.AssignedDock == null));

            if (filter.ExpectedArrivalFrom != null)
                query = query.Where(vvn => vvn.ExpectedArrival >= filter.ExpectedArrivalFrom);

            if (filter.ExpectedArrivalTo != null)
                query = query.Where(vvn => vvn.ExpectedArrival <= filter.ExpectedArrivalTo);

            // Pagination
            int pageCount = (int)Math.Ceiling((double)query.Count() / filter.PageSize);
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            List<VesselVisitNotification> result = query.ToList();

            // CClient side filter of latest notification decision status
            if (filter.Status == NotificationStatusFilter.Accepted)
                result = result.Where(vvn => vvn.GetLatestDecision() != null && vvn.GetLatestDecision()!.Status == NotificationDecisionStatus.Approved).ToList();
            else if (filter.Status == NotificationStatusFilter.Rejected)
                result = result.Where(vvn => vvn.GetLatestDecision() != null && vvn.GetLatestDecision()!.Status == NotificationDecisionStatus.Rejected).ToList();

            return Task.FromResult(Page<VesselVisitNotification>.Of(result, filter, pageCount));
        }
        catch
        {
            throw new PersistencyFailedException("Failed to filter vessel visit notifications from the database.");
        }
    }

    public Task DeleteAsync(VesselVisitNotification notification)
    {
        try
        {
            _context.VesselVisitNotifications.Remove(notification);
            return _context.SaveChangesAsync();
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException("Failed to delete vessel visit notification from the database.");
        }
    }

    public Task<List<VesselVisitNotification>> GetVesselVisitNotificationsOnDayAsync(DateTime day, uint daysAhead)
    {
        try
        {
            List<VesselVisitNotification> notifications = _context.VesselVisitNotifications
                .Where(vvn => vvn.NotificationDecisions.Any(nd => nd.AssignedDock != null))
                .Where(vvn => vvn.ExpectedArrival >= day && vvn.ExpectedArrival <= day.AddDays(daysAhead))
                .ToList();

            return Task.FromResult(notifications);
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve vessel visit notifications for the specified day from the database.");
        }
    }

    public async Task<VesselVisitDistributionDto> GetVesselVisitNotificationDistributionAsync()
    {
        try
        {
            var distribution = (await _context.VesselVisitNotifications
                .GroupBy(vvn => vvn.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync())
                .ToDictionary(x => x.Status, x => x.Count);

            int pendingCount = distribution.ContainsKey(NotificationStatus.ApprovalPending) ? distribution[NotificationStatus.ApprovalPending] : 0;
            int acceptedCount = _context.VesselVisitNotifications
                .Count(vvn => vvn.NotificationDecisions.Any(nd => nd.Status == NotificationDecisionStatus.Approved));
            int rejectedCount = _context.VesselVisitNotifications
                .Count(vvn => vvn.NotificationDecisions.Any(nd => nd.Status == NotificationDecisionStatus.Rejected));

            var dto = new VesselVisitDistributionDto
            {
                Pending = pendingCount,
                Accepted = acceptedCount,
                Rejected = rejectedCount
            };

            return dto;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve vessel visit notification distribution from the database.");
        }
    }

    public async Task<IEnumerable<string>> GetVesselVisitNotificationIdsAsync()
    {
        try
        {
            var ids = await _context.VesselVisitNotifications
                .Select(vvn => vvn.NotificationId.Value)
                .ToListAsync();
            return ids;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve vessel visit notification IDs from the database.");
        }
    }
}