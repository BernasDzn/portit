using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;
using NSwag.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class QualificationController : ControllerBase
{

	private readonly ILogger<QualificationController> _logger;
	private readonly ApiContext _context;

	public QualificationController(ApiContext context, ILogger<QualificationController> logger)
	{
		_context = context;
		_logger = logger;
	}

	[HttpGet(Name = "GetQualifications")]
	public ActionResult<IEnumerable<Qualification>> GetAll()
	{
		var quals = _context.Qualifications.ToList();
		var qualsDtos = quals.Select(qual => qual.ToDTO()).ToList();

		return Ok(qualsDtos);
	}

	[HttpPut("{designation}", Name = "UpdateQualification")]
	public IActionResult Update(string designation, QualificationDto qualDto)
	{
		Qualification? existingQual = _context.Qualifications.FirstOrDefault(q => q.QualificationName.Value.Equals(designation));
		if (existingQual == null)
			return NotFound("Qualification not found");

		existingQual.UpdateQualificationName(qualDto.QualificationName);
		_context.SaveChanges();
		return Ok();
	}
}