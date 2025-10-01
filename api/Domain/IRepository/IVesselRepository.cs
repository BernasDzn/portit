namespace Domain.IRepository;

using Api.Domain.Model;

public interface IVesselRepository : IGenericRepository<Vessel>
{
    Task<IEnumerable<Vessel>> GetVesselsAsync();
    Task<Vessel> GetVesselByNameAsync(string name);

    new Task<Vessel> Add(Vessel vessel);
    Task<Vessel> Update(Vessel vessel);
}