using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class QualificationController : ControllerBase
{
	
	private readonly ILogger<QualificationController> _logger;

	public QualificationController(ILogger<QualificationController> logger)
	{
		_logger = logger;
	}

	[HttpGet(Name = "GetQualification")]
	public IEnumerable<Qualification> Get()
	{
		throw new NotImplementedException();
	}

}