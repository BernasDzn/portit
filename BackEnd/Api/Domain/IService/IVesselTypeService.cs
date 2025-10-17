using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Domain.Entities;
using Api.Infrastructure.Utilities;

public interface IVesselTypeService
{
    public Task<IEnumerable<VesselTypeDto>> GetVesselTypes();
    public Task<VesselTypeDto> GetByName(string name);
    public Task<Page<VesselTypeDto>> FilterVesselTypes(VesselTypeFilter filter);
    public Task<VesselTypeDto> Add(VesselTypeDto vesselTypeDto);
    public Task<VesselTypeDto> Update(string id, VesselTypeDto vesselTypeDto);
}