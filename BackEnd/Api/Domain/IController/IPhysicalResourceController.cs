using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Api.Application.Controllers;

public interface IPhysicalResourceController
{
    public Task<ActionResult<IEnumerable<object>>> GetAll();
    public Task<ActionResult<object>> GetByCode([FromQuery] string code);
    public Task<ActionResult<Page<object>>> Filter([FromQuery] PhysicalResourceFilter filter);
    public Task<ActionResult<STSCraneDto>> AddSTSCrane(CreateSTSCraneDto stsCraneDto);
    public Task<ActionResult<YardCraneDto>> AddYardCrane(CreateYardCraneDto resourceDto);
    public Task<ActionResult<TruckDto>> AddTruck(CreateTruckDto resourceDto);
    public Task<ActionResult<STSCraneDto>> UpdateSTSCrane(string code, CreateSTSCraneDto crane);
    public Task<ActionResult<YardCraneDto>> UpdateYardCrane(string code, CreateYardCraneDto crane);
    public Task<ActionResult<TruckDto>> UpdateTruck(string code, CreateTruckDto truck);
    public Task<ActionResult> Deactivate(string code);
}