using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

public interface IVesselTypeController
{
    public Task<ActionResult<IEnumerable<VesselTypeDto>>> GetAll();
    public Task<ActionResult<Page<VesselTypeDto>>> Filter([FromQuery] VesselTypeFilter filter);
    public Task<ActionResult<VesselTypeDto>> Create(VesselTypeDto vesselTypeDto);
    public Task<ActionResult<VesselTypeDto>> Update(string name, VesselTypeDto vesselTypeDto);
}