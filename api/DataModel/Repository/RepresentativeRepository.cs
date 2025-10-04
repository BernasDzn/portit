namespace DataModel.Repository;

using Domain.IRepository;
using Api.Domain.Model;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Api.Models;
using Domain.Model.Generic;
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
            throw new PersistencyFailedException("Failed to select representatives by email");
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

    public async Task<Representative> GetByCitizenIdAsync(string citizenId)
    {
        try
        {
            Representative? representative = await _context.Representatives
                .FirstOrDefaultAsync(q => q.CitizenshipId.ToString().Equals(citizenId));
            if (representative == null)
            {
                throw new EntityNotFoundException($"Representative with citizen ID {citizenId} not found.");
            }
            return representative;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to select representatives by citizen ID");
        }
    }
}