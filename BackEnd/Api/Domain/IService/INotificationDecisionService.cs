using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Api.Application.Services;

public interface INotificationDecisionService
{
    public Task<IEnumerable<NotificationDecisionDto>> GetNotificationDecisions(string vesselVisitNotificationId);
    public Task<NotificationDecisionDto> Add(CreateNotificationDecisionDto notificationDecisionDto, string vesselVisitNotificationId);
}