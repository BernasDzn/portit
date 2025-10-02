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

	[HttpGet("{id}")]
	public async Task<ActionResult<QualificationDto>> Get(string id)

	{
		try
		{
			var qualificationDto = await _qualificationService.GetQualificationById(id);
			return Ok(qualificationDto);
		}
		catch (System.Exception)
		{
			return NotFound();
		}
	}

	[HttpPost(Name = "PostQualification")]
	public async Task<ActionResult<QualificationDto>> Create(QualificationDto qualDto)
	{
		try
		{
			var createdQual = await _qualificationService.Add(qualDto);
			if (createdQual == null)
				return BadRequest("Could not create qualification");

			return CreatedAtAction(nameof(Get), new { id = createdQual?.IdCode }, createdQual);
		}
		catch (System.Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpPut("{id}", Name = "UpdateQualification")]
	public async Task<ActionResult<QualificationDto>> Update(string id, QualificationDto qualDto)
	{
		try
		{	
			var updatedQual = await _qualificationService.Update(id, qualDto);
			if (updatedQual == null)
				return BadRequest("Could not update qualification");

			return Ok(updatedQual);
		}
		catch (System.Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}