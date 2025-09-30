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
		List<string> errors = new List<string>();
		var dockDto = await _dockService.GetDockByName(designation, errors);

		 if (dockDto == null)
			return NotFound(errors);

		return Ok(dockDto);
	}

	[HttpGet("searchByVesselType", Name = "GetDocksByVesselType")]
	public async Task<ActionResult<IEnumerable<DockDto>>> GetByVesselType([FromQuery] string vesselType)
	{
		List<string> errors = new List<string>();
		IEnumerable<DockDto>? docksDtos = await _dockService.GetDockByVesselType(vesselType, errors);

		if (docksDtos == null || !docksDtos.Any())
			return NotFound(errors);

		return Ok(docksDtos);
	}

	[HttpGet("searchByLocation", Name = "GetDocksByLocation")]
	public async Task<ActionResult<IEnumerable<DockDto>>> GetByLocation([FromQuery] string location)
	{
		List<string> errors = new List<string>();
		var dockDto = await _dockService.GetDockByLocation(location, errors);

		if (dockDto == null)
			return NotFound(errors);

		return Ok(dockDto);
	}

	[HttpPost(Name = "CreateDock")]
	public async Task<ActionResult<DockDto>> Create(DockDto dockDto)
	{
		List<string> errors = new List<string>();
		var createdDock = await _dockService.Add(dockDto, errors);

		if (createdDock == null)
			return BadRequest(errors);

		return CreatedAtAction(nameof(GetByName), new { name = createdDock?.Name }, createdDock);
	}

	[HttpPut("{name}", Name = "UpdateDock")]
	public IActionResult Update(string name, DockDto dockDto)
	{
		List<string> errors = new List<string>();

		var updatedDock = _dockService.Update(name, dockDto, errors);
		if (updatedDock == null)
			return BadRequest(errors);

		return Ok(updatedDock);
	}

}