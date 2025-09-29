using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DockController : ControllerBase
{
	
	private readonly ILogger<DockController> _logger;
	private readonly ApiContext	_context;

	public DockController(ApiContext context, ILogger<DockController> logger)
	{
		_context = context;
		_logger = logger;
	}

	[HttpGet(Name = "GetDocks")]
	public ActionResult<IEnumerable<Dock>> GetAll()
	{
		var vtypes = _context.Docks.ToList();
		var vtypesDtos = vtypes.Select(vtype => vtype.ToDTO()).ToList();

		return Ok(vtypesDtos);
	}

}