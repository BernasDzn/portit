using Api.Application.Exceptions;
using DAL;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataModel.Repository;

public class PhysicalResourceRepository : GenericRepository<PhysicalResource>, IPhysicalResourceRepository
{
    private new readonly ApiContext _context;
    public PhysicalResourceRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public Task<STSCrane> AddSTSCrane(STSCrane crane)
    {
        try
        {
            _context.PhysicalResources.Add(crane);
            _context.SaveChanges();
            return Task.FromResult(crane);
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException("Failed to add the STS crane to the database.");
        }
    }

    public Task<Truck> AddTruck(Truck truck)
    {
        try 
        {
            _context.PhysicalResources.Add(truck);
            _context.SaveChanges();
            return Task.FromResult(truck);
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException("Failed to add the truck to the database.");
        }
    }

    public Task<YardCrane> AddYardCrane(YardCrane crane)
    {
        try
        {
            _context.PhysicalResources.Add(crane);
            _context.SaveChanges();
            return Task.FromResult(crane);
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException("Failed to add the yard crane to the database.");
        }
    }

    public async Task<IEnumerable<object>> GetPhysicalResourcesAsync()
    {
        try
        {
            var allResources = new List<object>();

            allResources.AddRange(await _context.PhysicalResources.OfType<STSCrane>().ToListAsync());
            allResources.AddRange(await _context.PhysicalResources.OfType<YardCrane>().ToListAsync());
            allResources.AddRange(await _context.PhysicalResources.OfType<Truck>().ToListAsync());

            return allResources;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve physical resources from the database.");
        }
    }

    public async Task<PhysicalResource?> GetResourceByCodeAsync(string code)
    {
        try
        {
            PhysicalResource? resource = await _context.PhysicalResources.FirstOrDefaultAsync(r => r.Code.Value.Equals(code));
            return resource;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to retrieve the physical resource from the database.");
        }
    }

    public async Task<PhysicalResource> Update(PhysicalResource resource)
    {
        try
        {
            _context.PhysicalResources.Update(resource);
            await _context.SaveChangesAsync();
            return resource;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to update the physical resource in the database.");
        }
    }
}