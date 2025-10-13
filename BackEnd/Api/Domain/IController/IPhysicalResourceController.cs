using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface IPhysicalResourceController
{
    public Task<ActionResult<IEnumerable<object>>> GetAll();
    public Task<ActionResult<object>> GetByCode([FromQuery] string code);
    public Task<ActionResult<Page<object>>> Filter([FromQuery] PhysicalResourceFilter filter);
    public Task<ActionResult<STSCraneDto>> AddSTSCrane(STSCraneDto stsCraneDto);
    public Task<ActionResult<YardCraneDto>> AddYardCrane(YardCraneDto resourceDto);
    public Task<ActionResult<TruckDto>> AddTruck(TruckDto resourceDto);
    public Task<ActionResult<STSCraneDto>> UpdateSTSCrane(string code, STSCraneDto crane);
    public Task<ActionResult<YardCraneDto>> UpdateYardCrane(string code, YardCraneDto crane);
    public Task<ActionResult<TruckDto>> UpdateTruck(string code, TruckDto truck);
    public Task<ActionResult> Deactivate(string code);
}