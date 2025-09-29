using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;

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

	[HttpPost(Name = "CreateVesselType")]
	public ActionResult<VesselTypeDto> Create(VesselTypeDto vtypeDto)
	{
		var vtype = VesselType.FromDTO(vtypeDto);
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

	[HttpPut("{id}", Name = "UpdateVesselType")]
	public IActionResult Update(Guid id, VesselTypeDto vtypeDto)
	{
		if (id != vtypeDto.Id)
		{
			return BadRequest();
		}

		var vtype = _context.VesselTypes.Find(id);
		if (vtype == null)
		{
			return NotFound();
		}

		vtype = VesselType.FromDTO(vtypeDto);
		_context.VesselTypes.Update(vtype);
		_context.SaveChanges();
		return NoContent();
	}

}