namespace Domain.IRepository;

using Api.Domain.Model;

public interface IStorageAreaRepository : IGenericRepository<StorageArea>
{
    Task<IEnumerable<StorageArea>> GetStorageAreasAsync();
    Task<StorageArea?> GetStorageAreaByCodeAsync(string code);

    new Task<StorageArea> Add(StorageArea vessel);
    Task<StorageArea> Update(StorageArea vessel);
}