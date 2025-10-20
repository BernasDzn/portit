namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

[ApiController]
[Route("[controller]")]
public class PhysicalResourceController : ControllerBase, IPhysicalResourceController
{
    private readonly IPhysicalResourceService _physicalResourceService;
    private readonly ILogger<PhysicalResourceController> _logger;

    public PhysicalResourceController(IPhysicalResourceService physicalResourceService, ILogger<PhysicalResourceController> logger)
    {
        _physicalResourceService = physicalResourceService;
        _logger = logger;
    }

    [HttpGet(Name = "GetAll")]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        try
        {
            var allResources = await _physicalResourceService.GetPhysicalResources();
            return Ok(allResources);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error retrieving physical resources, {Message}", e.Message);
            return StatusCode(500, "An error occurred while retrieving physical resources.");
        }
    }

    [HttpGet("{code}", Name = "GetByCode")]
    public async Task<ActionResult<object>> GetByCode([FromQuery] string code)
    {
        try
        {
            var resource = await _physicalResourceService.GetResourceByCode(code);
            return Ok(resource);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error retrieving resource by id, {Message}", e.Message);
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error retrieving physical resource by code, {Message}", e.Message);
            return StatusCode(500, "An error occurred while retrieving the physical resource.");
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
        catch (System.Exception e)
        {
            _logger.LogCritical("Error filtering physical resources, {Message}", e.Message);
            return StatusCode(500, "An error occurred while filtering physical resources.");
        }
    }

    private async Task<ActionResult> HandleCreationAsync<TInput, TOutput>(TInput resourceDto, Func<TInput, Task<TOutput>> creationFunc, string resourceName)
        where TInput : class
        where TOutput : class
    {
        try
        {
            var createdResource = await creationFunc(resourceDto);

            return CreatedAtAction(nameof(GetByCode), new { code = (createdResource as dynamic).Code }, createdResource);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error retrieving dependency by id, {Message}", e.Message);
            return NotFound(e.Message);
        }
        catch (EntityAlreadyExistsException e)
        {
            _logger.LogError("Resource of code already exists, {Message}", e.Message);
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
            {
                _logger.LogError("Invalid argument provided for creating {ResourceName}, {Message}", resourceName, e.Message);
                return BadRequest(e.Message);
            }

            _logger.LogCritical("Error creating physical resource, {Message}", e.Message);
            return StatusCode(500, "An error occurred while creating the physical resource.");
        }
    }

    private async Task<ActionResult> HandleUpdateAsync<TInput, TOutput>(string code, TInput resourceDto, Func<string, TInput, Task<TOutput>> updateFunc, string resourceName)
        where TInput : class
        where TOutput : class
    {
        try
        {
            var updatedResource = await updateFunc(code, resourceDto);
            return NoContent();
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error retrieving dependency by id, {Message}", e.Message);
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
            {
                _logger.LogError("Invalid argument provided for updating {ResourceName}, {Message}", resourceName, e.Message);
                return BadRequest(e.Message);
            }

            _logger.LogCritical("Error updating physical resource, {Message}", e.Message);
            return StatusCode(500, "An error occurred while updating the physical resource.");
        }
    }

    [HttpDelete("{code}", Name = "Deactivate")]
    public async Task<ActionResult> Deactivate(string code)
    {
        try
        {
            var success = await _physicalResourceService.DeactivateResource(code);
            return NoContent();
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error retrieving resource by id, {Message}", e.Message);
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error deactivating physical resource, {Message}", e.Message);
            return StatusCode(500, "An error occurred while deactivating the physical resource.");
        }
    }

    [HttpPost("AddSTSCrane", Name = "AddSTSCrane")]
    public async Task<ActionResult<STSCraneDto>> AddSTSCrane([FromBody] CreateSTSCraneDto resourceDto) =>
        await HandleCreationAsync(resourceDto, _physicalResourceService.AddSTSCraneAsync, "STS crane");

    [HttpPost("AddYardCrane", Name = "AddYardCrane")]
    public async Task<ActionResult<YardCraneDto>> AddYardCrane([FromBody] CreateYardCraneDto resourceDto) =>
        await HandleCreationAsync(resourceDto, _physicalResourceService.AddYardCraneAsync, "yard crane");

    [HttpPost("AddTruck", Name = "AddTruck")]
    public async Task<ActionResult<TruckDto>> AddTruck([FromBody] CreateTruckDto resourceDto) =>
        await HandleCreationAsync(resourceDto, _physicalResourceService.AddTruckAsync, "truck");

    [HttpPut("UpdateSTSCrane/{code}", Name = "UpdateSTSCrane")]
    public async Task<ActionResult<STSCraneDto>> UpdateSTSCrane(string code, [FromBody] CreateSTSCraneDto resourceDto) =>
        await HandleUpdateAsync(code, resourceDto, _physicalResourceService.UpdateSTSCraneAsync, "STS crane");

    [HttpPut("UpdateYardCrane/{code}", Name = "UpdateYardCrane")]
    public async Task<ActionResult<YardCraneDto>> UpdateYardCrane(string code, [FromBody] CreateYardCraneDto resourceDto) =>
        await HandleUpdateAsync(code, resourceDto, _physicalResourceService.UpdateYardCraneAsync, "yard crane");

    [HttpPut("UpdateTruck/{code}", Name = "UpdateTruck")]
    public async Task<ActionResult<TruckDto>> UpdateTruck(string code, [FromBody] CreateTruckDto resourceDto) =>
        await HandleUpdateAsync(code, resourceDto, _physicalResourceService.UpdateTruckAsync, "truck");
}
