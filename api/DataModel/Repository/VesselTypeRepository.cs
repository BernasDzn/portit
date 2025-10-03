using Api.Domain.Model;
using DAL;
using DataModel.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Api.Application.Exceptions;

public class VesselTypeRepository : GenericRepository<VesselType>, IVesselTypeRepository
{
    private new readonly ApiContext _context;

    public VesselTypeRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VesselType>> GetVesselTypesAsync()
    {
        try
        {
            IEnumerable<VesselType> vtypes = await _context.VesselTypes.ToListAsync();

            return vtypes;
        }
        catch
        {
            throw;
        }
    }

    public async Task<VesselType> GetVesselTypeByNameAsync(string name)
    {
        try
        {
            VesselType? vtype = await _context.VesselTypes.FirstOrDefaultAsync(vt => vt.Name.Value.Equals(name));

            return vtype!;
        }
        catch
        {
            throw;
        }
    }
    
    public Task<Page<VesselType>> FilterVesselTypesAsync(VesselTypeFilter filter)
    {
        try
        {
            IQueryable<VesselType> query = _context.VesselTypes.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(vt => vt.Name.Value.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Description))
                query = query.Where(vt => vt.Description != null && vt.Description.Value.Contains(filter.Description));

            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

            return Task.FromResult(Page<VesselType>.Of(query.ToList(), filter));
        }
        catch
        {
            throw;
        }
    }

    public new async Task<VesselType> Add(VesselType vesselType)
    {
        try
        {
            _context.VesselTypes.Add(vesselType);
            await _context.SaveChangesAsync();
            return vesselType;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to select a vessel type by name");
        }
    }

    public async Task<bool> Update(VesselType vesselType)
    {
        try
        {
            _context.VesselTypes.Update(vesselType);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }
    }
}