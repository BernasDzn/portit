namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

[ApiController]
[Route("[controller]")]
public class StaffController : ControllerBase
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
			return BadRequest(e.Message);
		}
	}

	[HttpPut("{mecanographicNumber}", Name = "DeactivateStaff")]
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
			return BadRequest(e.Message);
		}
	}

	[HttpGet("filter")]
	public async Task<ActionResult<IEnumerable<StaffDto>>> Filter([FromQuery] StaffFilter filter)
	{ 
		try
		{
			var staffsDto = await _staffService.FilterStaffs(filter);
			return Ok(staffsDto);
		}
		catch (System.Exception)
		{
			return NotFound();
		}
	}

}