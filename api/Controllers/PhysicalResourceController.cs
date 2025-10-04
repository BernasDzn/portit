using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PhysicalResourceController : ControllerBase
{
    private readonly PhysicalResourceService _physicalResourceService;

    public PhysicalResourceController(PhysicalResourceService physicalResourceService)
    {
        _physicalResourceService = physicalResourceService;
    }

    [HttpGet(Name = "GetAll")]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var allResources = await _physicalResourceService.GetPhysicalResources();
        return Ok(allResources);
    }

    [HttpGet("GetByCode", Name = "GetByCode")]
    public async Task<ActionResult<object>> GetByCode([FromQuery] string code)
    {
        try
        {
            var resource = await _physicalResourceService.GetResourceByCode(code);
            if (resource == null)
                return NotFound($"No physical resource found with code: {code}");

            return Ok(resource);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("filter")]
	public async Task<ActionResult<Page<object>>> Filter([FromQuery] PhysicalResourceFilter filter)
	{
		try
		{
			var pagedResources = await _physicalResourceService.FilterPhysicalResources(filter);
            return Ok(pagedResources);
		}
		catch (System.Exception)
		{
			return NotFound();
		}
	}

    private async Task<ActionResult> HandleCreationAsync<T>(T resourceDto, Func<T, Task<T>> creationFunc, string resourceName) where T : class
    {
        try
        {
            var createdResource = await creationFunc(resourceDto);
            if (createdResource == null)
                return BadRequest($"Could not create {resourceName}.");

            return CreatedAtAction(nameof(GetByCode), new { code = (createdResource as dynamic).Code }, createdResource);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task<ActionResult> HandleUpdateAsync<T>(string code, T resourceDto, Func<string, T, Task<T>> updateFunc, string resourceName) where T : class
    {
        try
        {
            var updatedResource = await updateFunc(code, resourceDto);
            if (updatedResource == null)
                return NotFound($"No {resourceName} found with code: {code}");

            return Ok(updatedResource);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("AddSTSCrane", Name = "AddSTSCrane")]
    public async Task<ActionResult<STSCraneDto>> AddSTSCrane([FromBody] STSCraneDto resourceDto) =>
        await HandleCreationAsync<STSCraneDto>(resourceDto, _physicalResourceService.AddSTSCraneAsync, "STS crane");

    [HttpPost("AddYardCrane", Name = "AddYardCrane")]
    public async Task<ActionResult<YardCraneDto>> AddYardCrane([FromBody] YardCraneDto resourceDto) =>
        await HandleCreationAsync<YardCraneDto>(resourceDto, _physicalResourceService.AddYardCraneAsync, "yard crane");

    [HttpPost("AddTruck", Name = "AddTruck")]
    public async Task<ActionResult<TruckDto>> AddTruck([FromBody] TruckDto resourceDto) =>
        await HandleCreationAsync<TruckDto>(resourceDto, _physicalResourceService.AddTruckAsync, "truck");

    [HttpPut("UpdateSTSCrane/{code}", Name = "UpdateSTSCrane")]
    public async Task<ActionResult<STSCraneDto>> UpdateSTSCrane(string code, [FromBody] STSCraneDto resourceDto) =>
        await HandleUpdateAsync<STSCraneDto>(code, resourceDto, _physicalResourceService.UpdateSTSCraneAsync, "STS crane");

    [HttpPut("UpdateYardCrane/{code}", Name = "UpdateYardCrane")]
    public async Task<ActionResult<YardCraneDto>> UpdateYardCrane(string code, [FromBody] YardCraneDto resourceDto) =>
        await HandleUpdateAsync<YardCraneDto>(code, resourceDto, _physicalResourceService.UpdateYardCraneAsync, "yard crane");

    [HttpPut("UpdateTruck/{code}", Name = "UpdateTruck")]
    public async Task<ActionResult<TruckDto>> UpdateTruck(string code, [FromBody] TruckDto resourceDto) =>
        await HandleUpdateAsync<TruckDto>(code, resourceDto, _physicalResourceService.UpdateTruckAsync, "truck");
}
