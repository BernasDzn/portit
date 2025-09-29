using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class QualificationController : ControllerBase
{
	
	private readonly ILogger<QualificationController> _logger;
	private readonly ApiContext	_context;

	public QualificationController(ApiContext context, ILogger<QualificationController> logger)
	{
		_context = context;
		_logger = logger;
	}

	[HttpGet(Name = "GetQualifications")]
	public ActionResult<IEnumerable<Qualification>> GetAll()
	{
		return Ok(_context.Qualifications.ToList());
	}

}