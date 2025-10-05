namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;


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

	[HttpGet("filter")]
	public async Task<ActionResult<Page<QualificationDto>>> Filter([FromQuery] QualificationFilter filter)
	{
		try
		{
			var qualificationsDto = await _qualificationService.FilterQualifications(filter);
			return Ok(qualificationsDto);
		}
		catch (System.Exception)
		{
			return NotFound();
		}
	}

	[HttpGet("{id}", Name = "GetQualificationById")]
	public async Task<ActionResult<QualificationDto>> GetById(string id)
	{
		try
		{
			var qualDto = await _qualificationService.GetQualificationById(id);
			if (qualDto == null)
				return NotFound($"No qualification found with id: {id}");

			return Ok(qualDto);
		}
		catch (System.Exception e)
		{
			return BadRequest(e.Message);
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

			return CreatedAtAction(nameof(GetById), new { id = createdQual.IdCode }, createdQual);
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