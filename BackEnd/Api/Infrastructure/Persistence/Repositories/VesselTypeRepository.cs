namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

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
            throw new PersistencyFailedException("Failed to retrieve vessel types from the database.");
        }
    }

    public async Task<VesselType?> GetVesselTypeByNameAsync(string name)
    {
        try
        {
            VesselType? vtype = await _context.VesselTypes.FirstOrDefaultAsync(vt => vt.Name.Value.Equals(name));

            return vtype!;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to select a vessel type by name from the database.");
        }
    }
    
    public Task<Page<VesselType>> FilterVesselTypesAsync(VesselTypeFilter filter)
    {
        try
        {
            IQueryable<VesselType> query = _context.VesselTypes.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(vt => vt.Name.Value.ToLower().Contains(filter.Name.ToLower()));

            if (!string.IsNullOrEmpty(filter.Description))
                query = query.Where(vt => vt.Description.Value.ToLower().Contains(filter.Description.ToLower()));

            int pageCount = (int) Math.Ceiling((double)query.Count() / filter.PageSize);
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            return Task.FromResult(Page<VesselType>.Of(query.ToList(), filter, pageCount));
        }
        catch
        {
            throw new PersistencyFailedException("Failed to filter vessel types from the database.");
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
            throw new PersistencyFailedException("Failed to add vessel type to the database.");
        }
    }

    public async Task<VesselType> Update(VesselType vesselType)
    {
        try
        {
            _context.VesselTypes.Update(vesselType);
            await _context.SaveChangesAsync();
            return vesselType;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to update vessel type in the database.");
        }
    }
}