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
	private readonly ApiContext _context;

	public VesselTypeController(ApiContext context, ILogger<VesselTypeController> logger)
	{
		_context = context;
		_logger = logger;
	}

	[HttpGet(Name = "GetVesselTypes")]
	public ActionResult<IEnumerable<VesselType>> GetAll()
	{
		var vtypes = _context.VesselTypes.ToList();
		var vtypesDtos = vtypes.Select(vtype => vtype.ToDTO()).ToList();

		return Ok(vtypesDtos);
	}

	[HttpGet("searchByName", Name = "GetVesselTypesByName")]
	public ActionResult<IEnumerable<VesselType>> GetByName([FromQuery] string name)
	{
		var vtypes = _context.VesselTypes
			.Where(vtype => vtype.Name.Value.Contains(name, StringComparison.OrdinalIgnoreCase))
			.ToList();

		var vtypesDtos = vtypes.Select(vtype => vtype.ToDTO()).ToList();

		return Ok(vtypesDtos);
	}

	[HttpGet("searchByDescription", Name = "GetVesselTypesByDescription")]
	public ActionResult<IEnumerable<VesselType>> GetByDescription([FromQuery] string description)
	{
		var vtypes = _context.VesselTypes
			.Where(vtype => vtype.Description.Contains(description, StringComparison.OrdinalIgnoreCase))
			.ToList();
		var vtypesDtos = vtypes.Select(vtype => vtype.ToDTO()).ToList();
		return Ok(vtypesDtos);
	}

	[HttpPost(Name = "CreateVesselType")]
	public ActionResult<VesselTypeDto> Create(VesselTypeDto vtypeDto)
	{
		var vtype = new VesselType(Guid.NewGuid(), new Designation {Value = vtypeDto.Name}, vtypeDto.Description, vtypeDto.MaxNumberOfRows, vtypeDto.MaxNumberOfBays, vtypeDto.MaxNumberOfTiers);
		_context.VesselTypes.Add(vtype);
		_context.SaveChanges();
		return CreatedAtAction(nameof(GetAll), new { id = vtype.Id }, vtype.ToDTO());
	}

	[HttpDelete("{id}", Name = "DeleteVesselType")]
	public IActionResult Delete(Guid id)
	{
		var vtype = _context.VesselTypes.Find(id);
		if (vtype == null)
		{
			return NotFound();
		}

		_context.VesselTypes.Remove(vtype);
		_context.SaveChanges();
		return NoContent();
	}

	[HttpPut("{name}", Name = "UpdateVesselType")]
	public IActionResult Update(string name, VesselTypeDto vtypeDto)
	{
		if (name != vtypeDto.Name)
		{
			return BadRequest();
		}

		var vtype = _context.VesselTypes.FirstOrDefault(v => v.Name.Value == name);
		if (vtype == null)
		{
			return NotFound();
		}

		vtype.Update(vtypeDto);
		_context.VesselTypes.Update(vtype);
		_context.SaveChanges();
		return NoContent();
	}

}