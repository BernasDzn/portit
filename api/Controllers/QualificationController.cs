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

	public QualificationController(QualificationService qualificationService, ILogger<QualificationController> logger)
	{
		_qualificationService = qualificationService;
		_logger = logger;
	}

	[HttpGet(Name = "GetQualifications")]
	public async Task<ActionResult<IEnumerable<QualificationDto>>> GetAll()
	{
		IEnumerable<QualificationDto> qualificationsDto = await _qualificationService.GetQualifications();
		return Ok(qualificationsDto);
	}

	[HttpGet("{name}", Name = "GetQualificationByName")]
	public async Task<ActionResult<QualificationDto>> Get(string name)
	{
		List<string> errors = new List<string>();

		var qualificationDto = await _qualificationService.GetQualificationByName(name, errors);
		if (qualificationDto == null)
			return NotFound(errors);
		return Ok(qualificationDto);
	}

	[HttpPost(Name = "PostQualification")]
	public async Task<ActionResult<QualificationDto>> Create(QualificationDto qualDto)
	{
		List<string> errors = new List<string>();

		var createdQual = await _qualificationService.Add(qualDto, errors);
		if (createdQual == null)
			return BadRequest(errors);

		return CreatedAtAction(nameof(Get), new { name = createdQual?.QualificationName }, createdQual);
	}

	[HttpPut("{name}", Name = "UpdateQualification")]
	public async Task<ActionResult<QualificationDto>> Update(string name, QualificationDto qualDto)
	{
		List<string> errors = new List<string>();

		var updatedQual = await _qualificationService.Update(name, qualDto, errors);
		if (updatedQual == null)
			return BadRequest(errors);

		return Ok(updatedQual);
	}
}