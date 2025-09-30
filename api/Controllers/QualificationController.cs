using Microsoft.AspNetCore.Mvc;
using Api.Domain.Model;
using DAL;
using Application.Services;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class QualificationController : ControllerBase
{

	private readonly ILogger<QualificationController> _logger;
	private readonly QualificationService _qualificationService;
	List<string> errors = new List<string>();

	public QualificationController(QualificationService qualificationService, ILogger<QualificationController> logger)
	{
		_qualificationService = qualificationService;
		_logger = logger;
	}

	[HttpGet(Name = "GetQualifications")]
	public async Task<ActionResult<IEnumerable<QualificationDto>>> GetQualifications()
	{
		IEnumerable<QualificationDto> qualificationsDto = await _qualificationService.GetQualifications();
		return Ok(qualificationsDto);
	}

	[HttpPost(Name = "PostQualification")]
	public async Task<ActionResult<QualificationDto>> PostQualification(QualificationDto qualDto)
	{
		QualificationDto qualificationDto = await _qualificationService.Add(qualDto, errors);

		if (qualificationDto != null)
		{
			return CreatedAtAction(nameof(GetQualifications), new { id = qualificationDto.Id }, qualificationDto);
		}
		return BadRequest(errors);
	}

	[HttpPut("{id}", Name = "UpdateQualification")]
	public async Task<IActionResult> PutQualification(Guid id, QualificationDto qualDto)
	{
		bool wasUpdated = await _qualificationService.Update(id, qualDto, errors);
		if (!wasUpdated)
		{
			return BadRequest(errors);
		}

		return Ok();
	}
	
}