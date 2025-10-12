namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Application.Exceptions;

public class ContainerRepository : GenericRepository<Container>, IContainerRepository
{
    private new readonly ApiContext _context;
    public ContainerRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Container>> GetContainersAsync()
    {
        try
        {
            IEnumerable<Container> containers =  await _context.Containers.ToListAsync();
            return containers;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<Container> GetContainerByNumberAsync(string containerNumber)
    {
        try
        {
            Container? container = await _context.Containers.FirstOrDefaultAsync(c => c.ContainerNumber.Value.Equals(containerNumber));
            return container!;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    Task<Container> IContainerRepository.Add(Container container)
    {
        try
        {
            _context.Containers.Add(container);
            _context.SaveChanges();
            return Task.FromResult(container);
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException("Could not add container to database");
        }
    }

    public Task<Container> Update(Container container)
    {
        try
        {
            _context.Containers.Update(container);
            _context.SaveChanges();
            return Task.FromResult(container);
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException("Could not update container in database");
        }
    }
}
