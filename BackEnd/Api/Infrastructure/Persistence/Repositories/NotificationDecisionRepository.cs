namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
public class NotificationDecisionRepository : GenericRepository<NotificationDecision>, INotificationDecisionRepository
{
    private new readonly ApiContext _context;

    public NotificationDecisionRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationDecision>> GetNotificationDecisionsAsync()
    {
        try
        {
            IEnumerable<NotificationDecision> decisions = await _context.NotificationDecisions.ToListAsync();

            return decisions;
        }
        catch
        {
            throw;
        }
    }

    public async Task<NotificationDecision> GetNotificationDecisionByIdAsync(Guid id)
    {
        try
        {
            NotificationDecision? decision = await _context.NotificationDecisions.FirstOrDefaultAsync(nd => nd.Id == id);
            if (decision == null) throw new KeyNotFoundException($"NotificationDecision with ID {id} not found.");

            return decision;
        }
        catch
        {
            throw;
        }
    }


    public new async Task<NotificationDecision> Add(NotificationDecision notificationDecision)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Update(NotificationDecision notificationDecision)
    {
        throw new NotImplementedException();
    }
}