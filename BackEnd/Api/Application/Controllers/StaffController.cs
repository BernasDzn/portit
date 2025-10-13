namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

[ApiController]
[Route("[controller]")]
public class StaffController : ControllerBase, IStaffController
{
	private readonly ILogger<StaffController> _logger;
	private readonly StaffService _staffService;

	public StaffController(StaffService staffService, ILogger<StaffController> logger)
	{
		_staffService = staffService;
		_logger = logger;
	}

	[HttpGet(Name = "GetStaffs")]
	public async Task<ActionResult<IEnumerable<StaffDto>>> GetAll()
	{
		IEnumerable<StaffDto> staffsDto = await _staffService.GetStaffs();
		return Ok(staffsDto);
	}

	[HttpPost(Name = "PostStaff")]
	public async Task<ActionResult<StaffDto>> Create(StaffDto staffDto)
	{
		try
		{
			var createdStaff = await _staffService.Add(staffDto);
			if (createdStaff == null)
				return BadRequest("Could not create staff");

			return CreatedAtAction(nameof(GetAll), new { id = createdStaff?.MechanograficNumber }, createdStaff);
		}
		catch (Exception e)
		{
			_logger.LogError("Error creating staff, {Message}", e.Message);
			return BadRequest(e.Message);
		}
	}

	[HttpDelete("{mecanographicNumber}", Name = "DeactivateStaff")]
	public async Task<ActionResult> Deactivate(string mecanographicNumber)
	{
		try
		{
			StaffDto? deactivatedStaff = await _staffService.Deactivate(mecanographicNumber);
			if (deactivatedStaff == null)
				return BadRequest("Could not deactivate staff");
			return Ok(deactivatedStaff); 
		}
		catch (Exception e)
		{
			_logger.LogError("Error deactivating staff, {Message}", e.Message);
			return BadRequest(e.Message);
		}
	}

	[HttpPut("{mecanographicNumber}", Name = "UpdateStaff")]
	public async Task<ActionResult<StaffDto>> Update(string mecanographicNumber, StaffDto staffDto)
	{
		try
		{
			var updatedStaff = await _staffService.Update(mecanographicNumber, staffDto);
			if (updatedStaff == null)
				return BadRequest("Could not update staff");

			return Ok(updatedStaff);
		}
		catch (Exception e)
		{
			_logger.LogError("Error updating staff, {Message}", e.Message);
			return BadRequest(e.Message);
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
		catch (System.Exception e)
		{
			_logger.LogError("Error filtering staffs, {Message}", e.Message);
			return NotFound();
		}
	}

}