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

    public async Task<IEnumerable<Dock>> GetDockByVesselTypeAsync(string vesselType)
    {
        try
        {
            IEnumerable<Dock> docks = await _context.Docks
                .Where(d => d.SupportedVesselTypes.Any(vt => vt.Name.Value.Equals(vesselType)))
                .ToListAsync();
            return docks;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Dock> GetDockByLocationAsync(string location)
    {
        try
        {
            Dock? dock = await _context.Docks.FirstOrDefaultAsync(d => d.Location.Value.Equals(location));
            return dock!;
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
