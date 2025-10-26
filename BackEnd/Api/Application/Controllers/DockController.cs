namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

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
		try
		{
			var dockDto = await _dockService.GetByCode(code);
			return Ok(dockDto);
		}
		catch (EntityNotFoundException e)
		{
			_logger.LogError("Error retrieving dock by code, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (System.Exception e)
		{
			_logger.LogCritical("Error retrieving dock by code, {Message}", e.Message);
			return StatusCode(500, "An error occurred while retrieving the dock.");
		}
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
			return StatusCode(500, "An error occurred while filtering docks.");
		}
	}

	[HttpPost(Name = "CreateDock")]
	public async Task<ActionResult<DockDto>> Create(CreateDockDto dockDto)
	{
		try
		{
			var createdDock = await _dockService.Add(dockDto);

			return CreatedAtAction(nameof(GetAll), new { name = createdDock.Name }, createdDock);
		}
		catch (EntityAlreadyExistsException e)
		{
			_logger.LogError("Dock already exists, {Message}", e.Message);
			return Conflict(e.Message);
		}
		catch (EntityNotFoundException e)
		{
			_logger.LogError("Related entity not found when creating dock, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (System.Exception e)
		{

			if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
			{
				_logger.LogError("Invalid argument provided for creating dock, {Message}", e.Message);
				return BadRequest(e.Message);
			}

			_logger.LogError("Error creating dock, {Message}", e.Message);
			return StatusCode(500, "An error occurred while creating the dock.");
		}
	}

	[HttpPut("{code}", Name = "UpdateDock")]
	public async Task<ActionResult<DockDto>> Update(string code, CreateDockDto dockDto)
	{
		try
		{
			var updatedDock = await _dockService.Update(code, dockDto);

			return Ok(updatedDock);
		}
		catch (EntityNotFoundException e)
		{
			_logger.LogError("Entity not found when updating dock, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (System.Exception e)
		{
			if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
			{
				_logger.LogError("Invalid argument provided for updating {DockCode}, {Message}", code, e.Message);
				return BadRequest(e.Message);
			}

			_logger.LogError("Error updating dock, {Message}", e.Message);
			return StatusCode(500, "An error occurred while updating the dock.");
		}
	}

}