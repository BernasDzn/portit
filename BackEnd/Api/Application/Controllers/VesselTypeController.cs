namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;

[ApiController]
[Route("[controller]")]
public class VesselTypeController : ControllerBase, IVesselTypeController
{

	private readonly ILogger<VesselTypeController> _logger;
	private readonly IVesselTypeService _vesselTypeService;

	public VesselTypeController(IVesselTypeService vesselTypeService, ILogger<VesselTypeController> logger)
	{
		_vesselTypeService = vesselTypeService;
		_logger = logger;
	}

	[HttpGet(Name = "GetVesselTypes")]
	public async Task<ActionResult<IEnumerable<VesselTypeDto>>> GetAll()
	{
		IEnumerable<VesselTypeDto> vtypes = await _vesselTypeService.GetVesselTypes();
		return Ok(vtypes);
	}

	[HttpGet("{name}")]
	public async Task<ActionResult<VesselTypeDto>> GetByName(string name)
	{
		try
		{
			var vesselTypeDto = await _vesselTypeService.GetByName(name);

			if (vesselTypeDto == null)
				return NotFound($"No Vessel type found with name: {name}");

			return Ok(vesselTypeDto);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error getting vessel type by name, {Message}", e.Message);
			return BadRequest(e.Message);
		}
	}

	[HttpGet("filter")]
    public async Task<ActionResult<Page<VesselTypeDto>>> Filter([FromQuery] VesselTypeFilter filter)
    {
        try
		{
			var vesselTypesDtos = await _vesselTypeService.FilterVesselTypes(filter);

			return Ok(vesselTypesDtos);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error filtering vessel types, {Message}", e.Message);
			return StatusCode(500, "An error occurred while filtering vessel types.");
		}
    }
	[HttpPost(Name = "CreateVesselType")]
	public async Task<ActionResult<VesselTypeDto>> Create(VesselTypeDto vesselTypeDto)
	{
		try
		{	
			VesselTypeDto? vTypeDto = await _vesselTypeService.Add(vesselTypeDto);

			if (vTypeDto == null)
				return BadRequest("Cannot create vessel type");

			return CreatedAtAction(nameof(GetAll), new { name = vTypeDto.Name }, vTypeDto);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error creating vessel type, {Message}", e.Message);
			return BadRequest(e.Message);
		}
	}

	[HttpPut("{name}", Name = "UpdateVesselType")]
	public async Task<ActionResult<VesselTypeDto>> Update(string name, VesselTypeDto vesselTypeDto)
	{
		try
		{
			VesselTypeDto? vTypeDto = await _vesselTypeService.Update(name, vesselTypeDto);

			if (vTypeDto == null)
				return BadRequest("Could not update vessel type");

			return Ok(vTypeDto);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error updating vessel type, {Message}", e.Message);
			return BadRequest(e.Message);
		}
	}

}