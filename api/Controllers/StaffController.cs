using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;
using Application.Services;
using Api.Representations.DTO;

namespace Api.Controllers;

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
		catch (System.Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}