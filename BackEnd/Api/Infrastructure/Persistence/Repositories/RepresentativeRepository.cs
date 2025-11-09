namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;

public class RepresentativeRepository : GenericRepository<Representative>, IRepresentativeRepository
{
    private new readonly ApiContext _context = null!;

    public RepresentativeRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Representative> GetByEmailAsync(string email)
    {
        try
        {
            Console.WriteLine($"Searching for representative with email: {email}");
            Representative? representative = await _context.Representatives
                .FirstOrDefaultAsync(q => q.EmailAddress.Value.Equals(email));
            if (representative == null)
            {
                throw new EntityNotFoundException($"Representative with email {email} not found.");
            }
            return representative;
        }
        catch 
        {
            throw;
        }
    }

    public async Task<IEnumerable<Representative>> GetAllAsync()
    {
        try
        {
            IEnumerable<Representative> representatives = await _context.Representatives.ToListAsync();
            return representatives;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to select all representatives");
        }
    }

    public async Task<Representative> GetByCitizenIdAsync(uint citizenId)
    {
        try
        {
            Representative? representative = await _context.Representatives
                .FirstOrDefaultAsync(q => q.CitizenshipId == citizenId);
          
            return representative!;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to select representatives by citizen ID");
        }
    }
}