using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DockController : ControllerBase
{

	private readonly ILogger<DockController> _logger;
	private readonly ApiContext _context;

	public DockController(ApiContext context, ILogger<DockController> logger)
	{
		_context = context;
		_logger = logger;
	}

	[HttpGet(Name = "GetDocks")]
	public ActionResult<IEnumerable<Dock>> GetAll()
	{
		var docks = _context.Docks.ToList();
		var docksDtos = docks.Select(dock => dock.ToDTO()).ToList();

		return Ok(docksDtos);
	}

	[HttpGet("searchByName", Name = "GetDocksByName")]
	public ActionResult<IEnumerable<Dock>> GetByName([FromQuery] string designation)
	{
		var docks = _context.Docks
			.Where(dock => dock.Name.Value.Contains(designation, StringComparison.OrdinalIgnoreCase))
			.ToList();

		var docksDtos = docks.Select(dock => dock.ToDTO()).ToList();

		return Ok(docksDtos);
	}

	[HttpGet("searchByVesselType", Name = "GetDocksByVesselType")]
	public ActionResult<IEnumerable<Dock>> GetByVesselType([FromQuery] string vesselType)
	{
		var docks = _context.Docks
			.Where(dock => dock.SupportedVesselTypes.Any(vt => vt.Name.Value.Contains(vesselType, StringComparison.OrdinalIgnoreCase)))
			.ToList();

		var docksDtos = docks.Select(dock => dock.ToDTO()).ToList();

		return Ok(docksDtos);
	}

	[HttpGet("searchByLocation", Name = "GetDocksByLocation")]
	public ActionResult<IEnumerable<Dock>> GetByLocation([FromQuery] string location)
	{
		var docks = _context.Docks
			.Where(dock => dock.Location.Value.Contains(location, StringComparison.OrdinalIgnoreCase))
			.ToList();

		var docksDtos = docks.Select(dock => dock.ToDTO()).ToList();

		return Ok(docksDtos);
	}

	[HttpPost(Name = "CreateDock")]
	public ActionResult<DockDto> Create(DockDto dockDto)
	{
		var dock = Dock.FromDTO(dockDto);
		_context.Docks.Add(dock);
		_context.SaveChanges();
		return CreatedAtAction(nameof(GetAll), new { id = dock.Id }, dock.ToDTO());
	}

	[HttpDelete("{id}", Name = "DeleteDock")]
	public IActionResult Delete(Guid id)
	{
		var dock = _context.Docks.Find(id);
		if (dock == null)
		{
			return NotFound();
		}

		_context.Docks.Remove(dock);
		_context.SaveChanges();
		return NoContent();
	}

	[HttpPut("{id}", Name = "UpdateDock")]
	public IActionResult Update(Guid id, DockDto dockDto)
	{
		if (id != dockDto.Id)
		{
			return BadRequest();
		}

		var dock = _context.Docks.Find(id);
		if (dock == null)
		{
			return NotFound();
		}

		dock = Dock.FromDTO(dockDto);
		_context.Docks.Update(dock);
		_context.SaveChanges();
		return NoContent();
	}

}