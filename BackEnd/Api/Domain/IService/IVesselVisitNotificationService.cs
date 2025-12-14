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
    Task<IEnumerable<string>> GetAllAcceptedVVNIds();
    Task<VesselVisitNotificationDto> Add(CreateVesselVisitNotificationDto vesselVisitNotificationDto, string userEmail);
    Task<VesselVisitNotificationDto> Update(string vvnID, CreateVesselVisitNotificationDto vvnDTO, string userEmail);
    Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter, string userEmail);
    Task<Page<VesselVisitNotificationStatusDto>> FilterNotificationsPa(VesselVisitNotificationFilterPa filter);
    Task<IEnumerable<string>> GetVesselVisitNotificationIds();

    // Draft operations
    Task SubmitNotification(string id, string userEmail);
    Task DeleteNotificationDraft(string id, string userEmail);
    
    // Schedule operations
    Task<SchedulingResultDto> CollectSchedulingData(DateTime date, uint daysAhead);
    Task<VesselVisitDistributionDto> GetVesselVisitNotificationDistribution();
    
    // Dock rebalancing
    Task<DockRebalancingResponseDto> RebalanceDocks(DateTime date, uint daysAhead);
    Task ApplyDockRebalancing(DockRebalancingDto[] assignments);

    // Vessel Positions
    Task<IEnumerable<VesselPositionDto>> GetVesselPositionsAsync();
}