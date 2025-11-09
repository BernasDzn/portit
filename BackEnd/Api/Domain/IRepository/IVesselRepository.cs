namespace Api.Domain.IRepository;

using Api.Domain.Entities;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IVesselRepository : IGenericRepository<Vessel>
{
    Task<IEnumerable<Vessel>> GetVesselsAsync();
    Task<Vessel?> GetVesselByIMOAsync(string imo);
    Task<Page<Vessel>> FilterVesselsAsync(VesselFilter filter);
    new Task<Vessel> Add(Vessel vessel);
    Task<Vessel> Update(Vessel vessel);
    Task<int> CountAsync();
}