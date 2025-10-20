using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Api.Application.Services;

public interface IPhysicalResourceService
{
    public Task<IEnumerable<object>> GetPhysicalResources();
    public Task<object> GetResourceByCode(string code);
    public Task<STSCraneDto> AddSTSCraneAsync(CreateSTSCraneDto stsCraneDto);
    public Task<YardCraneDto> AddYardCraneAsync(CreateYardCraneDto resourceDto);
    public Task<TruckDto> AddTruckAsync(CreateTruckDto resourceDto);
    public Task<STSCraneDto> UpdateSTSCraneAsync(string code, CreateSTSCraneDto crane);
    public Task<YardCraneDto> UpdateYardCraneAsync(string code, CreateYardCraneDto crane);
    public Task<TruckDto> UpdateTruckAsync(string code, CreateTruckDto truck);
    public Task<Page<object>> FilterPhysicalResources(PhysicalResourceFilter filter);
    public Task<bool> DeactivateResource(string code);
}