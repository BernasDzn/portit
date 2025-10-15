using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IVesselService
{
    Task<IEnumerable<VesselDto>> GetVessels();
    Task<VesselDto?> Add(VesselDto vesselDto);
    Task<VesselDto?> Update(string imoNumber, VesselDto vesselDto);
}