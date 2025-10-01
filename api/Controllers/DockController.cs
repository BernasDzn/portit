using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DockController : ControllerBase
{

	private readonly ILogger<DockController> _logger;
	private readonly DockService _dockService;

	public DockController(DockService dockService, ILogger<DockController> logger)
	{
		_dockService = dockService;
		_logger = logger;
	}

	[HttpGet(Name = "GetDocks")]
	public async Task<ActionResult<IEnumerable<DockDto>>> GetAll()
	{
		IEnumerable<DockDto> docks = await _dockService.GetDocks();
		return Ok(docks);
	}

	[HttpGet("searchByName", Name = "GetDocksByName")]
	public async Task<ActionResult<IEnumerable<DockDto>>> GetByName([FromQuery] string designation)
	{
		DockDto? dockDto = await _dockService.GetDockByName(designation);

		if (dockDto == null)
			return NotFound();

		return Ok(dockDto);
	}

	[HttpGet("searchByVesselType", Name = "GetDocksByVesselType")]
	public async Task<ActionResult<IEnumerable<DockDto>>> GetByVesselType([FromQuery] string vesselType)
	{
		IEnumerable<DockDto>? docksDtos = await _dockService.GetDockByVesselType(vesselType);

		if (docksDtos == null || !docksDtos.Any())
			return NotFound();

		return Ok(docksDtos);
	}

	[HttpGet("searchByLocation", Name = "GetDocksByLocation")]
	public async Task<ActionResult<IEnumerable<DockDto>>> GetByLocation([FromQuery] string location)
	{
		DockDto? dockDto = await _dockService.GetDockByLocation(location);

		if (dockDto == null)
			return NotFound();

		return Ok(dockDto);
	}

	[HttpPost(Name = "CreateDock")]
	public async Task<ActionResult<DockDto>> Create(DockDto dockDto)
	{
		try
		{
			DockDto? createdDock = await _dockService.Add(dockDto);

			if (createdDock == null)
				return BadRequest();

			return CreatedAtAction(nameof(GetAll), new { name = createdDock?.Name }, createdDock);
		}
		catch (System.Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpPut("{name}", Name = "UpdateDock")]
	public async Task<IActionResult> Update(string name, DockDto dockDto)
	{
		try
		{
			DockDto? updatedDock = await _dockService.Update(name, dockDto);
			if (updatedDock == null)
				return BadRequest();

			return Ok(updatedDock);
		}
		catch (System.Exception e)
		{
			return BadRequest(e.Message);
		}
	}

}