namespace Api.Domain.IRepository;

using Api.Domain.Entities;

public interface IStorageAreaRepository : IGenericRepository<StorageArea>
{
    Task<IEnumerable<StorageArea>> GetStorageAreasAsync();
    Task<StorageArea?> GetStorageAreaByCodeAsync(string code);

    new Task<StorageArea> Add(StorageArea vessel);
    Task<StorageArea> Update(StorageArea vessel);
}