using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VesselTypeController : ControllerBase
{
	
	private readonly ILogger<VesselTypeController> _logger;
	private readonly ApiContext	_context;

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

}