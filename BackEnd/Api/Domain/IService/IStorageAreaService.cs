using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Api.Application.Services;

public interface IStorageAreaService
{
	Task<IEnumerable<StorageAreaDto>> GetStorageAreas();
	Task<StorageAreaDto> GetStorageAreaByCode(string code);
	Task<StorageAreaDto> CreateStorageArea(CreateStorageAreaDto createStorageAreaDto);
	Task<StorageAreaDto> UpdateStorageArea(string id, CreateStorageAreaDto updateStorageAreaDto);
	Task<Page<StorageAreaDto>> FilterStorageAreas(StorageAreaFilter filter);
	Task<int> CountStorageAreasAsync();
}
