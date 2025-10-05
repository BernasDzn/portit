namespace Api.Domain.IRepository;

using Api.Domain.Entities;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IDockRepository : IGenericRepository<Dock>
{
	Task<IEnumerable<Dock>> GetDocksAsync();
	Task<Dock> GetDockByNameAsync(string name);
	Task<Page<Dock>> FilterDocksAsync(DockFilter filter);
	new Task<Dock> Add(Dock dock);
	Task<bool> Update(Dock dock);
}