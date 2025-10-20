namespace Api.Domain.IRepository;

using Api.Domain.Entities;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IVesselTypeRepository : IGenericRepository<VesselType>
{
    Task<IEnumerable<VesselType>> GetVesselTypesAsync();
    Task<VesselType?> GetVesselTypeByNameAsync(string name);
    Task<Page<VesselType>> FilterVesselTypesAsync(VesselTypeFilter filter);
    new Task<VesselType> Add(VesselType vesselType);
    Task<VesselType> Update(VesselType vesselTypeDto);
}