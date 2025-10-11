namespace Api.Domain.IRepository;

using Api.Domain.Entities;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IContainerRepository : IGenericRepository<Container>
{
    Task<IEnumerable<Container>> GetContainersAsync();
    Task<Container> GetContainerByNumberAsync(string containerNumber);
    new Task<Container> Add(Container container);
    Task<Container> Update(Container container);
}