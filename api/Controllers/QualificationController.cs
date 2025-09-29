using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;

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

	[HttpPut("{id}", Name = "UpdateQualification")]
	public IActionResult Update(Guid id, QualificationDto qualDto)
	{
		Qualification? existingQual = _context.Qualifications.FirstOrDefault(q => q.Id == id);
		if (existingQual == null)
			return NotFound("Qualification not found");

		existingQual.UpdateQualificationName(qualDto.QualificationName);
		_context.SaveChanges();
		return Ok();
    }
}