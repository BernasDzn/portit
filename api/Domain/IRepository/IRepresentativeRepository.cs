namespace Domain.IRepository;

using Api.Domain.Model;
using Api.Models;
using Domain.Model.Generic;

public interface IRepresentativeRepository : IGenericRepository<Representative>
{
    Task<Representative> GetByEmailAsync(string email);
    Task<IEnumerable<Representative>> GetAllAsync();
    Task<Representative> GetByCitizenIdAsync(string citizenId);
}