namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;


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

	[HttpGet("filter")]
	public async Task<ActionResult<IEnumerable<DockDto>>> Filter([FromQuery] DockFilter filter)
	{
		try
		{
			var docksDtos = await _dockService.FilterDocks(filter);
			return Ok(docksDtos);
		}
		catch (System.Exception)
		{
			return NotFound();
		}
	}

	[HttpPost(Name = "CreateDock")]
	public async Task<ActionResult<DockDto>> Create(DockDto dockDto)
	{
		try
		{
			var createdDock = await _dockService.Add(dockDto);

			if (createdDock == null)
				return BadRequest();

			return CreatedAtAction(nameof(GetAll), new { name = createdDock.Name }, createdDock);
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
			var updatedDock = await _dockService.Update(name, dockDto);
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