namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;


[ApiController]
[Route("[controller]")]
public class DockController : ControllerBase, IDockController
{
	private readonly ILogger<DockController> _logger;
	private readonly IDockService _dockService;

	public DockController(IDockService dockService, ILogger<DockController> logger)
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

	[HttpGet("{code}", Name = "GetDockByCode")]
	public async Task<ActionResult<DockDto>> GetByCode(string code)
	{
		DockDto? dock = await _dockService.GetByCode(code);
		if (dock == null)
			return NotFound();

		return Ok(dock);
	}

	[HttpGet("filter")]
	public async Task<ActionResult<IEnumerable<DockDto>>> Filter([FromQuery] DockFilter filter)
	{
		try
		{
			var docksDtos = await _dockService.FilterDocks(filter);
			return Ok(docksDtos);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error filtering docks, {Message}", e.Message);
			return NotFound();
		}
	}

	[HttpPost(Name = "CreateDock")]
	public async Task<ActionResult<DockDto>> Create(CreateDockDto dockDto)
	{
		try
		{
			var createdDock = await _dockService.Add(dockDto);

			if (createdDock == null)
				return BadRequest("Unable to create dock");

			return CreatedAtAction(nameof(GetAll), new { name = createdDock.Name }, createdDock);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error creating dock, {Message}", e.Message);
			return BadRequest(e.Message);
		}
	}

	[HttpPut("{name}", Name = "UpdateDock")]
	public async Task<ActionResult<DockDto>> Update(string name, CreateDockDto dockDto)
	{
		try
		{
			var updatedDock = await _dockService.Update(name, dockDto);
			if (updatedDock == null)
				return BadRequest("Could not update dock");

			return Ok(updatedDock);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error updating dock, {Message}", e.Message);
			return BadRequest(e.Message);
		}
	}

}