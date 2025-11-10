namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "VesselType.Manage")]
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

			return Ok(vesselTypeDto);
		}
		catch (EntityNotFoundException e)
		{
			_logger.LogError("Error retrieving vessel type by name, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (System.Exception e)
		{
			_logger.LogCritical("Error retrieving vessel type by name, {Message}", e.Message);
			return StatusCode(500, "An error occurred while retrieving the vessel type.");
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
			var createdVesselType = await _vesselTypeService.Add(vesselTypeDto);

			return CreatedAtAction(nameof(GetAll), new { name = createdVesselType.Name }, createdVesselType);
		}
		catch (EntityAlreadyExistsException e)
		{
			_logger.LogError("Vessel Type already exists, {Message}", e.Message);
			return Conflict(e.Message);
		}
		catch (System.Exception e)
		{

			if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
			{
				_logger.LogError("Invalid argument provided for creating vessel type, {Message}", e.Message);
				return BadRequest(e.Message);
			}

			_logger.LogError("Error creating vessel type, {Message}", e.Message);
			return StatusCode(500, "An error occurred while creating the vessel type.");
		}
	}

	[HttpPut("{name}", Name = "UpdateVesselType")]
	public async Task<ActionResult<VesselTypeDto>> Update(string name, VesselTypeDto vesselTypeDto)
	{
		try
		{
			var updatedVesselType = await _vesselTypeService.Update(name, vesselTypeDto);

			return Ok(updatedVesselType);
		}
		catch (EntityNotFoundException e)
		{
			_logger.LogError("Entity not found when updating vessel type, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (EntityAlreadyExistsException e)
		{
			_logger.LogError("Vessel Type already exists when updating, {Message}", e.Message);
			return Conflict(e.Message);
		}
		catch (System.Exception e)
		{
			if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
			{
				_logger.LogError("Invalid argument provided for updating {VesselTypeName}, {Message}", name, e.Message);
				return BadRequest(e.Message);
			}

			_logger.LogError("Error updating vessel type, {Message}", e.Message);
			return StatusCode(500, "An error occurred while updating the vessel type.");
		}
	}

	[HttpGet("count")]
	public async Task<ActionResult<int>> Count()
	{
		try
		{
			var count = await _vesselTypeService.CountVesselTypesAsync();
			return Ok(count);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error counting vessel types, {Message}", e.Message);
			return StatusCode(500);
		}
	}

}