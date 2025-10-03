using Api.Domain.Model;

namespace Domain.IRepository;

public interface IDockRepository : IGenericRepository<Dock>
{
    Task<IEnumerable<Dock>> GetDocksAsync();
	Task<Dock> GetDockByNameAsync(string name);
    Task<Page<Dock>> FilterDocksAsync(DockFilter filter);
	new Task<Dock> Add(Dock dock);
	Task<bool> Update(Dock dock);
}