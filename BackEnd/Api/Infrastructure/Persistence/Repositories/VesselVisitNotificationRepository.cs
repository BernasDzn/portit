namespace Api.Infrastructure.Persistence.Repositories;

using System.Collections;
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

    public Task<Page<VesselVisitNotification>> FilterVesselVisitNotificationsAsync(VesselVisitNotificationFilter filter)
    {
        try
        {
            Representative? submitter = _context.Representatives.FirstOrDefault(rep => rep.CitizenshipId == filter.SubmitterCitizeshipId);
            if (submitter == null)
                throw new EntityNotFoundException($"No Representative found with Citizenship ID {filter.SubmitterCitizeshipId}");
            
            if (submitter.RepresentedOrganization == null)
                throw new EntityNotFoundException($"The representative with Citizenship ID {filter.SubmitterCitizeshipId} does not represent any organization.");

            IQueryable<VesselVisitNotification> query = _context.VesselVisitNotifications.AsQueryable();
            ShippingAgentOrganization relatedOrg = _context.ShippingAgentOrganizations.FirstOrDefault(org => org.Representatives.Any(rep => rep.CitizenshipId == filter.SubmitterCitizeshipId)) 
                ?? throw new EntityNotFoundException($"No Shipping Agent Organization found for Submitter Citizenship ID {filter.SubmitterCitizeshipId}");

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
                    case NotificationStatusFilter.Accpeted:
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
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            List<VesselVisitNotification> result = query.ToList();

            // CLient side filter of latest notification decision status
            if (filter.Status == NotificationStatusFilter.Accpeted)
                result = result.Where(vvn => vvn.GetLatestDecision() != null && vvn.GetLatestDecision()!.Status == NotificationDecisionStatus.Approved).ToList();
            else if (filter.Status == NotificationStatusFilter.Rejected)
                result = result.Where(vvn => vvn.GetLatestDecision() != null && vvn.GetLatestDecision()!.Status == NotificationDecisionStatus.Rejected).ToList();

            return Task.FromResult(Page<VesselVisitNotification>.Of(result, filter));
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
}