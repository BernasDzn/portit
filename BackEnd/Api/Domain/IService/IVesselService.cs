using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IVesselService
{
    Task<IEnumerable<VesselDto>> GetVessels();
    Task<VesselDto> Add(CreateVesselDto vesselDto);
    Task<VesselDto> Update(string imoNumber, CreateVesselDto vesselDto);
    Task<VesselDto> GetByImo(string imo);
    Task<Page<VesselDto>> FilterVessels(VesselFilter filter);
    Task<int> CountVesselsAsync();
    Task<IEnumerable<VesselDto>> GetVesselByOwner(string ownerEmail);
}