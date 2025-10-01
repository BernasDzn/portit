namespace Domain.IRepository;

using Api.Domain.Model;

public interface IVesselTypeRepository : IGenericRepository<Vessel>
{
    Task<VesselType> GetVesselTypeByNameAsync(string name);
}