using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface IVesselController
{
    public Task<ActionResult<IEnumerable<VesselDto>>> GetAll();
    public Task<ActionResult<VesselDto>> GetByImo(string imo);
    public Task<ActionResult<VesselDto>> Create(VesselDto vesselDto);
    public Task<ActionResult<Page<VesselDto>>> Filter([FromQuery] VesselFilter filter);
}