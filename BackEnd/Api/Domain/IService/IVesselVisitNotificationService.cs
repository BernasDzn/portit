using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
namespace Api.Application.Services;

public interface IVesselVisitNotificationService
{
    Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications();
    Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotificationsOnDay(DateTime day, uint daysAhead);
    Task<VesselVisitNotificationDto> GetById(string vvnID);
    Task<VesselVisitNotificationDto> Add(CreateVesselVisitNotificationDto vesselVisitNotificationDto, string userEmail);
    Task<VesselVisitNotificationDto> Update(string vvnID, CreateVesselVisitNotificationDto vvnDTO, string userEmail);
    Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter, string userEmail);
    Task<Page<VesselVisitNotificationStatusDto>> FilterNotificationsPa(VesselVisitNotificationFilterPa filter);

    // Draft operations
    Task SubmitNotification(string id, string userEmail);
    Task DeleteNotificationDraft(string id, string userEmail);
    
    // Schedule operations
    Task<SchedulingResultDto> CollectSchedulingData(DateTime date, uint daysAhead, Code dockCode);
    Task<VesselVisitDistributionDto> GetVesselVisitNotificationDistribution();
}