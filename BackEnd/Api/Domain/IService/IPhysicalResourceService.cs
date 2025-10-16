using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Api.Application.Services;

public interface IPhysicalResourceService
{
    public Task<IEnumerable<object>> GetPhysicalResources();
    public Task<object> GetResourceByCode(string code);
    public Task<STSCraneDto> AddSTSCraneAsync(STSCraneDto stsCraneDto);
    public Task<YardCraneDto> AddYardCraneAsync(YardCraneDto resourceDto);
    public Task<TruckDto> AddTruckAsync(TruckDto resourceDto);
    public Task<STSCraneDto> UpdateSTSCraneAsync(string code, STSCraneDto crane);
    public Task<YardCraneDto> UpdateYardCraneAsync(string code, YardCraneDto crane);
    public Task<TruckDto> UpdateTruckAsync(string code, TruckDto truck);
    public Task<Page<object>> FilterPhysicalResources(PhysicalResourceFilter filter);
    public Task<bool> DeactivateResource(string code);
}