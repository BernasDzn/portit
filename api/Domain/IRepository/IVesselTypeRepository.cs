<<<<<<< HEAD
using Api.Domain.Model;
using Domain.IRepository;

public interface IVesselTypeRepository : IGenericRepository<VesselType>
{
    Task<IEnumerable<VesselType>> GetVesselTypesAsync();
    Task<VesselType> GetVesselTypeByNameAsync(string name);
    Task<VesselType> GetVesselTypeByDescriptionAsync(string description);
    new Task<VesselType> Add(VesselType vesselType);
    Task<bool> Update(string name, VesselTypeDto vesselTypeDto);
=======
namespace Domain.IRepository;

using Api.Domain.Model;

public interface IVesselTypeRepository : IGenericRepository<Vessel>
{
    Task<VesselType> GetVesselTypeByNameAsync(string name);
>>>>>>> 164c671bb7a852f65fe08a69607cc3deb501440d
}