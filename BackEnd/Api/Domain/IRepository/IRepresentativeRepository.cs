namespace Api.Domain.IRepository;

using Api.Domain.Entities;

public interface IRepresentativeRepository : IGenericRepository<Representative>
{
    Task<Representative> GetByEmailAsync(string email);
    Task<IEnumerable<Representative>> GetAllAsync();
    Task<Representative> GetByCitizenIdAsync(uint citizenId);
}