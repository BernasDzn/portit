using Api.Domain.Model;

namespace Domain.IRepository;

public interface IDockRepository : IGenericRepository<Dock>
{
    Task<IEnumerable<Dock>> GetDocksAsync();
	Task<Dock> GetDockByNameAsync(string name);
    Task<IEnumerable<Dock>> GetDockByVesselTypeAsync(string vesselType);
    Task<Dock> GetDockByLocationAsync(string location);
	new Task<Dock> Add(Dock dock);
	Task<bool> Update(string name, DockDto dockDto);
}