using Api.Domain.Model;
using Domain.IRepository;

public interface IVesselTypeRepository : IGenericRepository<VesselType>
{
    Task<IEnumerable<VesselType>> GetVesselTypesAsync();
    Task<VesselType> GetVesselTypeByNameAsync(string name);
    Task<Page<VesselType>> FilterVesselTypesAsync(VesselTypeFilter filter);
    new Task<VesselType> Add(VesselType vesselType);
    Task<bool> Update(VesselType vesselTypeDto);
}