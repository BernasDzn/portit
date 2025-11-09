using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IVesselVisitNotificationService
{
    Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications();
    Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotificationsOnDay(DateTime day);
    Task<VesselVisitNotificationDto> GetById(string vvnID);
    Task<VesselVisitNotificationDto> Add(CreateVesselVisitNotificationDto vesselVisitNotificationDto, string userEmail);
    Task<VesselVisitNotificationDto> Update(string vvnID, CreateVesselVisitNotificationDto vvnDTO, string userEmail);
    Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter, string userEmail);
    Task SubmitNotification(string id, string userEmail);
    Task DeleteNotificationDraft(string id, string userEmail);
}