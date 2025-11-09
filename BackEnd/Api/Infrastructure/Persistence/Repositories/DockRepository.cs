namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Application.Exceptions;

public class DockRepository : GenericRepository<Dock>, IDockRepository
{
    private new readonly ApiContext _context;
    public DockRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Dock>> GetDocksAsync()
    {
        try
        {
            IEnumerable<Dock> docks = await _context.Docks.ToListAsync();
            return docks;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve docks from the database.");
        }
    }

    public async Task<Dock?> GetDockByCodeAsync(string code)
    {
        try
        {
            Dock? dock = await _context.Docks.FirstOrDefaultAsync(d => d.Code.Value.Equals(code));
            return dock;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve dock by code from the database.");
        }
    }

    public Task<Page<Dock>> FilterDocksAsync(DockFilter filter)
    {
        try
        {
            IQueryable<Dock> query = _context.Docks.AsQueryable();

            if (!string.IsNullOrEmpty(filter.DockName))
                query = query.Where(d => d.Name.Value.ToLower().Contains(filter.DockName.ToLower()));

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(d => d.Location.Value.ToLower().Contains(filter.Location.ToLower()));

            if (!string.IsNullOrEmpty(filter.VesselTypeName))
                query = query.Where(d => d.SupportedVesselTypes.Any(vt => vt.Name.Value.ToLower().Equals(filter.VesselTypeName.ToLower())));

            int pageCount = (int) Math.Ceiling((double)query.Count() / filter.PageSize);
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            return Task.FromResult(Page<Dock>.Of(query.ToList(), filter, pageCount));
        }
        catch
        {
            throw new PersistencyFailedException("Failed to filter docks from the database.");
        }
    }

    public new async Task<Dock> Add(Dock dock)
    {
        try
        {
            _context.Docks.Add(dock);
            await _context.SaveChangesAsync();
            return dock;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to add dock to the database.");
        }
    }

    public async Task<Dock> Update(Dock dock)
    {
        try
        {
            _context.Docks.Update(dock);
            await _context.SaveChangesAsync();
            return dock;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to update dock in the database.");
        }
    }

    public async Task<int> CountAsync()
    {
        try
        {
            return await _context.Docks.CountAsync();
        }
        catch
        {
            throw new PersistencyFailedException("Failed to count docks in the database.");
        }
    }

}
