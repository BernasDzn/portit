namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public class VesselRepository : GenericRepository<Vessel>, IVesselRepository
{
    private new readonly ApiContext _context = null!;

    public VesselRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vessel>> GetVesselsAsync()
    {
        try
        {
            IEnumerable<Vessel> vessels = await _context.Vessels.ToListAsync();
            return vessels;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Vessel> GetVesselByIMOAsync(string imo)
    {
        try
        {
            Vessel? vessel = await _context.Vessels
                .FirstOrDefaultAsync(q => q.ImoIdentifier != null && q.ImoIdentifier.Value == imo);
            return vessel!;
        }
        catch
        {
            throw;
        }
    }

    public new async Task<Vessel> Add(Vessel vessel)
    {
        try
        {
            _context.Vessels.Add(vessel);
            await _context.SaveChangesAsync();
            return vessel;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Vessel> Update(Vessel vessel)
    {
        try
        {
            _context.Vessels.Update(vessel);
            await _context.SaveChangesAsync();
            return vessel;
        }
        catch
        {
            throw;
        }
    }

    public Task<Page<Vessel>> FilterVesselsAsync(VesselFilter filter)
    {
        try
        {
            IQueryable<Vessel> query = _context.Vessels.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(v => v.Name.Value.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.ImoNumber))
                query = query.Where(v => v.ImoIdentifier != null && v.ImoIdentifier.Value.Contains(filter.ImoNumber));

            if (!string.IsNullOrEmpty(filter.TaxNumber))
                query = query.Where(v => v.Owner != null && v.Owner.TaxId.Value != null && v.Owner.TaxId.Value.Contains(filter.TaxNumber));

            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

            return Task.FromResult(Page<Vessel>.Of(query.ToList(), filter));
        }
        catch
        {
            throw;
        }
    }
}