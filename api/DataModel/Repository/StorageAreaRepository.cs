using Api.Application.Exceptions;
using Api.Domain.Model;
using DAL;
using DataModel.Repository;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

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

    public Task<StorageArea> Update(StorageArea vessel)
    {
        throw new NotImplementedException();
    }

    Task<StorageArea> IStorageAreaRepository.Add(StorageArea vessel)
    {
        throw new NotImplementedException();
    }
}