namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "Vessel.Manage")]
public class VesselController : ControllerBase, IVesselController
{
    private readonly ILogger<VesselController> _logger;
    private readonly IVesselService _vesselService;

    public VesselController(IVesselService vesselService, ILogger<VesselController> logger)
    {
        _vesselService = vesselService;
        _logger = logger;
    }

    [HttpGet(Name = "GetVessels")]
    public async Task<ActionResult<IEnumerable<VesselDto>>> GetAll()
    {
        IEnumerable<VesselDto> vesselsDto = await _vesselService.GetVessels();
        return Ok(vesselsDto);
    }

    [HttpGet("{imo}", Name = "GetVesselByImo")]
    public async Task<ActionResult<VesselDto>> GetByImo(string imo)
    {
        try
        {
            VesselDto? vesselDto = await _vesselService.GetByImo(imo);
            return Ok(vesselDto);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Vessel not found, {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error retrieving vessel by IMO, {Message}", e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPost(Name = "PostVessel")]
    public async Task<ActionResult<VesselDto>> Create(CreateVesselDto vesselDto)
    {
        try
        {
            var createdVessel = await _vesselService.Add(vesselDto);

            return CreatedAtAction(nameof(GetAll), new { name = createdVessel.Name }, createdVessel);
        }
        catch (EntityAlreadyExistsException ex)
        {
            _logger.LogWarning("Vessel already exists, {Message}", ex.Message);
            return Conflict(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Referenced entity not found, {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (PersistencyFailedException ex)
        {
            _logger.LogError("Persistency failed, {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error creating vessel, {Message}", e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpGet("filter")]
    public async Task<ActionResult<Page<VesselDto>>> Filter([FromQuery] VesselFilter filter)
    {
        try
        {
            var vesselDtos = await _vesselService.FilterVessels(filter);

            return Ok(vesselDtos);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error filtering vessels, {Message}", e.Message);
            return NotFound();
        }
    }

    [HttpPut("{imo}", Name = "UpdateVessel")]
    public async Task<ActionResult<VesselDto>> Update(string imo, CreateVesselDto vesselDto)
    {
        try
        {
            var updatedVessel = await _vesselService.Update(imo, vesselDto);

            return Ok(updatedVessel);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error updating vessel, {Message}", e.Message);
            return BadRequest(e.Message);
        }
    }
}
