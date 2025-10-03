using Api.Application.Exceptions;
using Api.Domain.Model;
using DAL;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataModel.Repository;

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
            throw;
        }
    }

    public async Task<Dock> GetDockByNameAsync(string name)
    {
        try
        {
            Dock? dock = await _context.Docks.FirstOrDefaultAsync(d => d.Name.Value.Equals(name));
            return dock!;
        }
        catch
        {
            throw;
        }
    }

    public Task<Page<Dock>> FilterDocksAsync(DockFilter filter)
    {
        try
        {
            IQueryable<Dock> query = _context.Docks.AsQueryable();

            if (!string.IsNullOrEmpty(filter.DockName))
                query = query.Where(d => d.Name.Value.Contains(filter.DockName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(d => d.Location.Value.Contains(filter.Location, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.VesselTypeName))
                query = query.Where(d => d.SupportedVesselTypes.Any(vt => vt.Name.Value.Contains(filter.VesselTypeName, StringComparison.OrdinalIgnoreCase)));

            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

            return Task.FromResult(Page<Dock>.Of(query.ToList(), filter));
        }
        catch
        {
            throw;
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
            throw;
        }
    }

    public async Task<bool> Update(Dock dock)
    {
        try
        {
            _context.Docks.Update(dock);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }
    }

}
