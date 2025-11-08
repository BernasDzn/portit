using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IVesselVisitNotificationService
{
    Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications();
    Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotificationsOnDay(DateTime day);
    Task<VesselVisitNotificationDto> GetById(string vvnID);
    Task<VesselVisitNotificationDto> Add(CreateVesselVisitNotificationDto vesselVisitNotificationDto);
    Task<VesselVisitNotificationDto> Update(string vvnID, CreateVesselVisitNotificationDto vvnDTO);
    Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter);
    Task SubmitNotification(string id);
    Task DeleteNotificationDraft(string id);
}