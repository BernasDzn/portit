namespace Api.Domain.IRepository;

using Api.Domain.Entities;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IPhysicalResourceRepository : IGenericRepository<PhysicalResource>
{
	Task<IEnumerable<PhysicalResource>> GetPhysicalResourcesAsync();
	Task<PhysicalResource?> GetResourceByCodeAsync(string code);
	Task<Page<PhysicalResource>> FilterPhysicalResourcesAsync(PhysicalResourceFilter filter);
	Task<int> CountAsync();
	Task<STSCrane> AddSTSCrane(STSCrane crane);
	Task<YardCrane> AddYardCrane(YardCrane crane);
	Task<Truck> AddTruck(Truck truck);

	Task<PhysicalResource> Update(PhysicalResource resource);
	Task<STSCrane> UpdateSTSCrane(STSCrane crane);
	Task<YardCrane> UpdateYardCrane(YardCrane crane);
	Task<Truck> UpdateTruck(Truck truck);
    Task<IEnumerable<STSCrane>> GetSTSCranesByDockCodeAsync(string value);
    Task<IEnumerable<STSCrane>> GetSTSCranesByDockCodesAsync(IEnumerable<string> enumerable);
}