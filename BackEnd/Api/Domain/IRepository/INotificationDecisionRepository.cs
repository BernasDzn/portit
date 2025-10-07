using Api.Domain.Entities;

namespace Api.Domain.IRepository;

public interface INotificationDecisionRepository : IGenericRepository<NotificationDecision>
{
    Task<IEnumerable<NotificationDecision>> GetNotificationDecisionsAsync();
    Task<NotificationDecision> GetNotificationDecisionByIdAsync(Guid id);
    new Task<NotificationDecision> Add(NotificationDecision notificationDecision);
    Task<bool> Update(NotificationDecision notificationDecision);
}