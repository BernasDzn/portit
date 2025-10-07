namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;

[ApiController]
[Route("[controller]")]
public class VesselTypeController : ControllerBase
{

	private readonly ILogger<VesselTypeController> _logger;
	private readonly VesselTypeService _vesselTypeService;

	public VesselTypeController(VesselTypeService vesselTypeService, ILogger<VesselTypeController> logger)
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
			return NotFound();
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
	public async Task<IActionResult> Update(string name, VesselTypeDto vesselTypeDto)
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