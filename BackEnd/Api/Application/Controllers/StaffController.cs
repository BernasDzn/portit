namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "Staff.Manage")]
public class StaffController : ControllerBase, IStaffController
{
	private readonly ILogger<StaffController> _logger;
	private readonly IStaffService _staffService;

	public StaffController(IStaffService staffService, ILogger<StaffController> logger)
	{
		_staffService = staffService;
		_logger = logger;
	}

	[HttpGet(Name = "GetStaffs")]
	public async Task<ActionResult<IEnumerable<StaffDto>>> GetAll()
	{
		IEnumerable<StaffDto> staffsDto;
		try
		{
			staffsDto = await _staffService.GetStaffs();
			return Ok(staffsDto);
		}
		catch (Exception ex)
		{
			_logger.LogError("Unexpected error getting staffs, {Message}", ex.Message);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpPost(Name = "PostStaff")]
	public async Task<ActionResult<StaffDto>> Create(CreateStaffDto staffDto)
	{
		try
		{
			var createdStaff = await _staffService.Create(staffDto);
			return CreatedAtAction(nameof(GetAll), new { id = createdStaff?.MechanographicNumber }, createdStaff);
		}
		catch (EntityNotFoundException ex)
		{
			_logger.LogError("Error creating staff, {Message}", ex.Message);
			return NotFound(ex.Message);
		}
		catch (EntityAlreadyExistsException ex)
		{
			_logger.LogError("Error creating staff, {Message}", ex.Message);
			return Conflict(ex.Message);
		}
		catch (Exception e)
		{
			if (e is ArgumentException || e is ArgumentNullException)
			{
				_logger.LogError("Validation error creating staff, {Message}", e.Message);
				return BadRequest(e.Message);
			}
			_logger.LogError("Error creating staff, {Message}", e.Message);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpDelete("{mecanographicNumber}", Name = "DeactivateStaff")]
	public async Task<ActionResult> Deactivate(string mecanographicNumber)
	{
		try
		{
			StaffDto deactivatedStaff = await _staffService.Deactivate(mecanographicNumber);
			return Ok(deactivatedStaff); 
		}catch (EntityNotFoundException e)
		{
			_logger.LogError("Error deactivating staff, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (Exception e)
		{
			if (e is ArgumentException || e is ArgumentNullException)
			{
				_logger.LogError("Validation error deactivating staff, {Message}", e.Message);
				return BadRequest(e.Message);
			}
			_logger.LogError("Error deactivating staff, {Message}", e.Message);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpPut("{mecanographicNumber}", Name = "UpdateStaff")]
	public async Task<ActionResult<StaffDto>> Update(string mecanographicNumber, CreateStaffDto staffDto)
	{
		try
		{
			var updatedStaff = await _staffService.Update(mecanographicNumber, staffDto);
			return Ok(updatedStaff);
		}catch (EntityNotFoundException e)
		{
			_logger.LogError("Error updating staff, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (Exception e)
		{
			if (e is ArgumentException || e is ArgumentNullException)
			{
				_logger.LogError("Validation error updating staff, {Message}", e.Message);
				return BadRequest(e.Message);
			}
			_logger.LogError("Error updating staff, {Message}", e.Message);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpGet("filter")]
	public async Task<ActionResult<Page<StaffDto>>> Filter([FromQuery] StaffFilter filter)
	{
		try
		{
			var staffsDto = await _staffService.FilterStaffs(filter);
			return Ok(staffsDto);
		}
		catch (Exception ex)
		{
			if (ex is ArgumentException || ex is ArgumentNullException)
			{
				_logger.LogError($"Validation error:{ex.Message}");
				return BadRequest(ex.Message);
			}
			_logger.LogError($"Something went wrong:{ex.Message}");
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpGet("count")]
	public async Task<ActionResult<int>> Count()
	{
		try
		{
			var count = await _staffService.CountStaffsAsync();
			return Ok(count);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error counting staffs, {Message}", e.Message);
			return StatusCode(500);
		}
	}

}