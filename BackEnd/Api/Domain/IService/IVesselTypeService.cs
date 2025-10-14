using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IVesselTypeService
{
    Task<IEnumerable<VesselTypeDto>> GetVesselTypes();
    Task<Page<VesselTypeDto>> FilterVesselTypes(VesselTypeFilter filter);
    Task<VesselTypeDto?> Add(VesselTypeDto vesselTypeDto);
    Task<VesselTypeDto?> Update(string id, VesselTypeDto vesselTypeDto);
}