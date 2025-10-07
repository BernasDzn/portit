namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Domain.IRepository;

public class NotificationDecisionService
{
    private readonly INotificationDecisionRepository _notificationDecisionRepository;

    public NotificationDecisionService(INotificationDecisionRepository notificationDecisionRepository)
    {
        _notificationDecisionRepository = notificationDecisionRepository;
    }

    public async Task<IEnumerable<NotificationDecisionDto>> GetNotificationDecisions()
    {
        var decisions = await _notificationDecisionRepository.GetNotificationDecisionsAsync();
        return decisions.Select(n => n.ToDTO()).ToList();
    }

}