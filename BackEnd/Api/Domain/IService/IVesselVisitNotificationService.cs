using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IVesselVisitNotificationService
{
    Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications();
    Task<VesselVisitNotificationDto?> Add(VesselVisitNotificationDto vesselVisitNotificationDto);
    Task<VesselVisitNotificationDto?> Update(string vvnID, VesselVisitNotificationDto vvnDTO);
    Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter);
}