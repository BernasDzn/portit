using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;
using Domain.Model.Generic;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VesselTypeController : ControllerBase
{

	private readonly ILogger<VesselTypeController> _logger;
	private readonly VesselTypeService _vesselTypeService;

	public VesselTypeController(VesselTypeService vesselTypeService, ILogger<VesselTypeController> logger)
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

	[HttpGet("searchByName", Name = "GetVesselTypesByName")]
	public async Task<ActionResult<VesselTypeDto>> GetByName([FromQuery] string name)
	{

		VesselTypeDto? vtype = await _vesselTypeService.GetVesselTypeByName(name);
		if (vtype == null)
		{
			return NotFound();
		}

		return Ok(vtype);
	}

	[HttpGet("searchByDescription", Name = "GetVesselTypesByDescription")]
	public async Task<ActionResult<VesselTypeDto>> GetByDescription([FromQuery] string description)
	{

		VesselTypeDto? vtype = await _vesselTypeService.GetVesselTypeByDescription(description);

		if (vtype == null)
		{
			return NotFound();
		}

		return Ok(vtype);
	}

	[HttpPost(Name = "CreateVesselType")]
	public async Task<ActionResult<VesselTypeDto>> Create(VesselTypeDto vesselTypeDto)
	{
		VesselTypeDto? vTypeDto = await _vesselTypeService.Add(vesselTypeDto);

		if (vTypeDto == null)
			return BadRequest();

		return CreatedAtAction(nameof(GetAll), new { name = vTypeDto.Name }, vTypeDto);
	}

	[HttpPut("{name}", Name = "UpdateVesselType")]
	public async Task<IActionResult> Update(string name, VesselTypeDto vesselTypeDto)
	{

		VesselTypeDto? vTypeDto = await _vesselTypeService.Update(name, vesselTypeDto);

		if (vTypeDto == null)
			return BadRequest();

		return Ok(vTypeDto);
	}

}