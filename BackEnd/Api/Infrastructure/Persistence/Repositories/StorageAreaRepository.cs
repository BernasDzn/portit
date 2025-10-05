namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;

public class StorageAreaRepository : GenericRepository<StorageArea>, IStorageAreaRepository
{
    private new readonly ApiContext _context = null!;

    public StorageAreaRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StorageArea>> GetStorageAreasAsync()
    {
        try
        {
            IEnumerable<StorageArea> storageAreas = await _context.StorageAreas.ToListAsync();
            return storageAreas;
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException("Failed to retrieve storage areas from the database.");
        }
    }

    public async Task<StorageArea?> GetStorageAreaByCodeAsync(string code)
    {
        try
        {
            StorageArea? storageArea = await _context.StorageAreas
                .FirstOrDefaultAsync(q => q.NameCode.Value.Equals(code));
            return storageArea;
        }
        catch (System.Exception)
        {
            throw new PersistencyFailedException($"Failed to retrieve storage area with code {code} from the database.");
        }
    }

    public Task<StorageArea> Update(StorageArea storageArea)
    {
        try
        {
            _context.StorageAreas.Update(storageArea);
            _context.SaveChanges();
            return Task.FromResult(storageArea);
        }
        catch
        {
            throw;
        }
    }

    public new async Task<StorageArea> Add(StorageArea storageArea)
    {
        try
        {
            _context.StorageAreas.Add(storageArea);
            await _context.SaveChangesAsync();
            return storageArea;
        }
        catch
        {
            throw;
        }
    }
}