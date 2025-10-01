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

    public async Task<VesselType> GetVesselTypeByDescriptionAsync(string description)
    {
        try
        {
            VesselType? vtype = await _context.VesselTypes.FirstOrDefaultAsync(vt => vt.Description.Value.Equals(description));

            return vtype!;
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

    public async Task<bool> Update(string name, VesselTypeDto vesselTypeDto)
    {
        try
        {
            VesselType vtype = await GetVesselTypeByNameAsync(name);
            if (vtype == null)
                throw new Exception("Vessel Type not found.");

            vtype.Update(vesselTypeDto);
            _context.VesselTypes.Update(vtype);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }
    }
}