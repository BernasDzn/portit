namespace DataModel.Repository;

using Domain.IRepository;
using Api.Domain.Model;
using DAL;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Vessel> GetVesselByNameAsync(string name)
    {
        try
        {
            Vessel? vessel = await _context.Vessels
                .FirstOrDefaultAsync(q => q.Name.Value.Equals(name));
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

    public async Task<bool> Update(string name, VesselDto vesselDto, List<string> errorMessage)
    {
        try
        {
            Vessel? vessel = await GetVesselByNameAsync(name);
            if (vessel == null)
            {
                errorMessage.Add("Vessel not found.");
                return false;
            }

            bool exists = await _context.Vessels
                .AnyAsync(q => q.Name.Value.Equals(vesselDto.Name) && !q.Name.Value.Equals(name));

            if (exists)
            {
                errorMessage.Add("Another vessel with the same name already exists.");
                return false;
            }

            vessel.Update(vesselDto);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }
    }
}