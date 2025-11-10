namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "Qualification.Manage")]
public class QualificationController : ControllerBase, IQualificationController
{
	private readonly ILogger<QualificationController> _logger;
	private readonly IQualificationService _qualificationService;

	public QualificationController(IQualificationService qualificationService, ILogger<QualificationController> logger)
	{
		_qualificationService = qualificationService;
		_logger = logger;
	}

	[HttpGet(Name = "GetQualifications")]
	public async Task<ActionResult<IEnumerable<QualificationDto>>> GetAll()
	{
		try
		{
			IEnumerable<QualificationDto> qualificationsDto = await _qualificationService.GetQualifications();
			return Ok(qualificationsDto);
		}
		catch (System.Exception)
		{
			_logger.LogCritical("Error retrieving qualifications");
			return StatusCode(500, "An error occurred while retrieving qualifications.");
		}
	}

	[HttpGet("filter")]
	public async Task<ActionResult<Page<QualificationDto>>> Filter([FromQuery] QualificationFilter filter)
	{
		try
		{
			var qualificationsDto = await _qualificationService.FilterQualifications(filter);
			return Ok(qualificationsDto);
		}
		catch (System.Exception e)
		{
			_logger.LogCritical("Error filtering qualifications, {Message}", e.Message);
			return StatusCode(500, "An error occurred while filtering qualifications.");
		}
	}

	[HttpGet("{id}", Name = "GetQualificationById")]
	public async Task<ActionResult<QualificationDto>> GetById(string id)
	{
		try
		{
			var qualDto = await _qualificationService.GetQualificationById(id);
			return Ok(qualDto);
		}
		catch (EntityNotFoundException e)
		{
			_logger.LogError("Error retrieving qualification by id, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (System.Exception e)
		{
			_logger.LogCritical("Error retrieving qualification by id, {Message}", e.Message);
			return StatusCode(500, "An error occurred while retrieving the qualification.");
		}
	}

	[HttpPost(Name = "PostQualification")]
	public async Task<ActionResult<QualificationDto>> Create(QualificationDto qualDto)
	{
		try
		{
			var createdQual = await _qualificationService.Add(qualDto);
			return CreatedAtAction(nameof(GetById), new { id = createdQual.IdCode }, createdQual);
		}
		catch (EntityAlreadyExistsException e)
		{
			_logger.LogError("Entity already exists, {Message}", e.Message);
			return Conflict(e.Message);
		}
		catch (System.Exception e)
		{
			if (e is ArgumentException || e is ArgumentNullException)
			{
				_logger.LogError("Invalid arguments provided for creating qualification, {Message}", e.Message);
				return BadRequest(e.Message);
			}

			_logger.LogCritical("Error creating qualification, {Message}", e.Message);
			return StatusCode(500, "An error occurred while creating the qualification.");
		}
	}

	[HttpPut("{id}", Name = "UpdateQualification")]
	public async Task<ActionResult> Update(string id, QualificationDto qualDto)
	{
		try
		{	
			var updatedQual = await _qualificationService.Update(id, qualDto);
			return NoContent();
		}
		catch (EntityNotFoundException e)
		{
			_logger.LogError("Error updating qualification, {Message}", e.Message);
			return NotFound(e.Message);
		}
		catch (System.Exception e)
		{
			if (e is ArgumentException || e is ArgumentNullException)
			{
				_logger.LogError("Invalid arguments provided for updating qualification, {Message}", e.Message);
				return BadRequest(e.Message);
			}

			_logger.LogCritical("Error updating qualification, {Message}", e.Message);
			return StatusCode(500, "An error occurred while updating the qualification.");
		}
	}

	[HttpGet("count")]
	public async Task<ActionResult<int>> Count()
	{
		try
		{
			var count = await _qualificationService.CountQualificationsAsync();
			return Ok(count);
		}
		catch (System.Exception e)
		{
			_logger.LogError("Error counting qualifications, {Message}", e.Message);
			return StatusCode(500);
		}
	}
}