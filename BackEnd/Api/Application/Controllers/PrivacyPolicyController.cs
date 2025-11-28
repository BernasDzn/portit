namespace Api.Application.Controllers;

using Api.Application.DataTransfer;
using Api.Domain.IController;
using Api.Domain.IService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PrivacyPolicyController : ControllerBase, IPrivacyPolicyController
{
	private readonly IPrivacyPolicyService _privacyPolicyService;
	private readonly ILogger<PrivacyPolicyController> _logger;

	public PrivacyPolicyController(
		IPrivacyPolicyService privacyPolicyService,
		ILogger<PrivacyPolicyController> logger)
	{
		_privacyPolicyService = privacyPolicyService;
		_logger = logger;
	}

	[HttpGet("active", Name = "GetActivePrivacyPolicy")]
	public async Task<ActionResult<PrivacyPolicyDto>> GetActive()
	{
		try
		{
			var privacyPolicy = await _privacyPolicyService.GetActivePrivacyPolicy();
			
			if (privacyPolicy == null)
				return NotFound(new { message = "No active privacy policy found." });

			return Ok(privacyPolicy);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving active privacy policy");
			return StatusCode(500, new { message = "An error occurred while retrieving the privacy policy." });
		}
	}

	[HttpGet(Name = "GetAllPrivacyPolicies")]
	public async Task<ActionResult<IEnumerable<PrivacyPolicyDto>>> GetAll()
	{
		try
		{
			var privacyPolicies = await _privacyPolicyService.GetAllPrivacyPolicies();
			return Ok(privacyPolicies);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving privacy policies");
			return StatusCode(500, new { message = "An error occurred while retrieving privacy policies." });
		}
	}

	[HttpGet("{id}", Name = "GetPrivacyPolicyById")]
	public async Task<ActionResult<PrivacyPolicyDto>> GetById(Guid id)
	{
		try
		{
			var privacyPolicy = await _privacyPolicyService.GetPrivacyPolicyById(id);
			
			if (privacyPolicy == null)
				return NotFound(new { message = $"Privacy policy with ID {id} not found." });

			return Ok(privacyPolicy);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving privacy policy {Id}", id);
			return StatusCode(500, new { message = "An error occurred while retrieving the privacy policy." });
		}
	}

	[HttpPost(Name = "CreatePrivacyPolicy")]
	public async Task<ActionResult<PrivacyPolicyDto>> Create(CreatePrivacyPolicyDto createDto)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(createDto.Content))
				return BadRequest(new { message = "Privacy policy content cannot be empty." });

			var createdPolicy = await _privacyPolicyService.CreatePrivacyPolicy(createDto);
			return CreatedAtAction(nameof(GetById), new { id = createdPolicy.Id }, createdPolicy);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating privacy policy");
			return StatusCode(500, new { message = "An error occurred while creating the privacy policy." });
		}
	}
}
