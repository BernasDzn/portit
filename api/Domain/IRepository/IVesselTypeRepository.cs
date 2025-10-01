using Api.Domain.Model;
using Domain.IRepository;

public interface IVesselTypeRepository : IGenericRepository<VesselType>
{
    Task<IEnumerable<VesselType>> GetVesselTypesAsync();
    Task<VesselType> GetVesselTypeByNameAsync(string name);
    Task<VesselType> GetVesselTypeByDescriptionAsync(string description);
    new Task<VesselType> Add(VesselType vesselType);
    Task<bool> Update(string name, VesselTypeDto vesselTypeDto);
}