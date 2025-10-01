namespace DataModel.Repository;

using Domain.IRepository;
using Api.Domain.Model;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class VesselTypeRepository : GenericRepository<Vessel>, IVesselTypeRepository
{
    private new readonly ApiContext _context = null!;

    public VesselTypeRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<VesselType?> GetVesselTypeByNameAsync(string name)
    {
        try
        {
            VesselType? vesselType = _context.VesselTypes
                .FirstOrDefaultAsync(q => q.Name.Value.Equals(name)).Result;
            return vesselType;
        }
        catch
        {
            throw;
        }
    }
}