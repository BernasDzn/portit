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

    [HttpPost("AddSTSCrane", Name = "AddSTSCrane")]
    public async Task<ActionResult<STSCraneDto>> AddSTSCrane([FromBody] STSCraneDto resourceDto)
    {
        try
        {
            var createdResource = await _physicalResourceService.AddSTSCraneAsync(resourceDto);
            if (createdResource == null)
                return BadRequest("Could not create the STS crane.");

            return CreatedAtAction(nameof(GetByCode), new { code = createdResource.Code }, createdResource);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("AddYardCrane", Name = "AddYardCrane")]
    public async Task<ActionResult<YardCraneDto>> AddYardCrane([FromBody] YardCraneDto resourceDto)
    {
        try
        {
            var createdResource = await _physicalResourceService.AddYardCraneAsync(resourceDto);
            if (createdResource == null)
                return BadRequest("Could not create the yard crane.");

            return CreatedAtAction(nameof(GetByCode), new { code = createdResource.Code }, createdResource);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("AddTruck", Name = "AddTruck")]
    public async Task<ActionResult<TruckDto>> AddTruck([FromBody] TruckDto resourceDto)
    {
        try
        {
            var createdResource = await _physicalResourceService.AddTruckAsync(resourceDto);
            if (createdResource == null)
                return BadRequest("Could not create the truck.");

            return CreatedAtAction(nameof(GetByCode), new { code = createdResource.Code }, createdResource);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
