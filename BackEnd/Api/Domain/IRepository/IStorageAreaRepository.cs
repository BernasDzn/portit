namespace Api.Domain.IRepository;

using Api.Application.DataTransfer.Filters;
using Api.Domain.Entities;
using Api.Infrastructure.Utilities;

public interface IStorageAreaRepository : IGenericRepository<StorageArea>
{
    Task<IEnumerable<StorageArea>> GetStorageAreasAsync();
    Task<StorageArea?> GetStorageAreaByCodeAsync(string code);

    new Task<StorageArea> Add(StorageArea vessel);
    Task<StorageArea> Update(StorageArea vessel);

    Task<Page<StorageArea>> FilterStorageAreasAsync(StorageAreaFilter filter);
}