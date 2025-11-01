namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Application.Exceptions;

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
            throw new PersistencyFailedException("Failed to retrieve vessels from the database.");
        }
    }

    public async Task<Vessel?> GetVesselByIMOAsync(string imo)
    {
        try
        {
            Vessel? vessel = await _context.Vessels
                .FirstOrDefaultAsync(q => q.ImoIdentifier != null && q.ImoIdentifier.Value == imo);
            return vessel;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve vessel by IMO from the database.");
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
            throw new PersistencyFailedException("Failed to add vessel to the database.");
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
            throw new PersistencyFailedException("Failed to update vessel in the database.");
        }
    }

    public Task<Page<Vessel>> FilterVesselsAsync(VesselFilter filter)
    {
        try
        {
            IQueryable<Vessel> query = _context.Vessels.AsQueryable();
            int pageCount = (int) Math.Ceiling((double)query.Count() / filter.PageSize);

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(v => v.Name.Value.ToLower().Contains(filter.Name.ToLower()));

            if (!string.IsNullOrEmpty(filter.ImoNumber))
                query = query.Where(v => v.ImoIdentifier.Value.ToLower().Contains(filter.ImoNumber.ToLower()));

            if (!string.IsNullOrEmpty(filter.TaxNumber))
                query = query.Where(v => v.Owner.TaxId.Value.ToLower().Contains(filter.TaxNumber.ToLower()));

            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            return Task.FromResult(Page<Vessel>.Of(query.ToList(), filter, pageCount));
        }
        catch
        {
            throw new PersistencyFailedException("Failed to filter vessels from the database.");
        }
    }
}