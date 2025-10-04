using Api.Domain.Model;

namespace Domain.IRepository;

public interface IPhysicalResourceRepository : IGenericRepository<PhysicalResource>
{
	Task<IEnumerable<object>> GetPhysicalResourcesAsync();
	Task<PhysicalResource?> GetResourceByCodeAsync(string code);

	Task<STSCrane> AddSTSCrane(STSCrane crane);
	Task<YardCrane> AddYardCrane(YardCrane crane);
	Task<Truck> AddTruck(Truck truck);
	Task<STSCrane> UpdateSTSCrane(STSCrane crane);
	Task<YardCrane> UpdateYardCrane(YardCrane crane);
	Task<Truck> UpdateTruck(Truck truck);
}