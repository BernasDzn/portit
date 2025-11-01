namespace Api.Infrastructure.Persistence.Repositories;

using Api.Application.DataTransfer.Filters;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;
using Api.Infrastructure.Utilities;

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

    public async Task<IEnumerable<PhysicalResource>> GetPhysicalResourcesAsync()
    {
        try
        {
            var allResources = new List<PhysicalResource>();
            allResources.AddRange(await _context.PhysicalResources.Where(r => r.Active).ToListAsync());

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
            PhysicalResource? resource = await _context.PhysicalResources.FirstOrDefaultAsync(r => r.Code.Value.Equals(code) && r.Active);
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

    public Task<STSCrane> UpdateSTSCrane(STSCrane crane)
    {
        STSCrane updatedCrane = (STSCrane) Update(crane).Result;
        return Task.FromResult(updatedCrane);
    }

    public Task<Truck> UpdateTruck(Truck truck)
    {
        Truck updatedTruck = (Truck) Update(truck).Result;
        return Task.FromResult(updatedTruck);
    }

    public Task<YardCrane> UpdateYardCrane(YardCrane crane)
    {
        YardCrane updatedCrane = (YardCrane) Update(crane).Result;
        return Task.FromResult(updatedCrane);
    }

    public Task<Page<PhysicalResource>> FilterPhysicalResourcesAsync(PhysicalResourceFilter filter)
    {
        IQueryable<PhysicalResource> query = _context.PhysicalResources.AsQueryable();
        query = query.Where(r => r.Active);

        if (!string.IsNullOrEmpty(filter.Code))
            query = query.Where(r => r.Code.Value.ToLower().Contains(filter.Code.ToLower()));

        if (!string.IsNullOrEmpty(filter.Description))
            query = query.Where(r => r.Description.Value.ToLower().Contains(filter.Description.ToLower()));

        if (filter.Status != null)
            query = query.Where(r => r.Status == filter.Status);

        if (filter.Type != null)
        {
            switch (filter.Type)
            {
                case PhysicalResourceFilter.ResourceType.STSCrane:
                    query = query.OfType<STSCrane>();
                    break;
                case PhysicalResourceFilter.ResourceType.YardCrane:
                    query = query.OfType<YardCrane>();
                    break;
                case PhysicalResourceFilter.ResourceType.Truck:
                    query = query.OfType<Truck>();
                    break;
            }
        }

        int pageCount = (int) Math.Ceiling((double)query.Count() / filter.PageSize);
        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
		return Task.FromResult(
			Page<PhysicalResource>.Of(query.ToList(), filter, pageCount)
		);
    }
}