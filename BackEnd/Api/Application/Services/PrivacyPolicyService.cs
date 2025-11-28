namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.IService;

public class PrivacyPolicyService : IPrivacyPolicyService
{
	private readonly IPrivacyPolicyRepository _privacyPolicyRepository;
	private readonly ILogger<PrivacyPolicyService> _logger;

	public PrivacyPolicyService(
		IPrivacyPolicyRepository privacyPolicyRepository,
		ILogger<PrivacyPolicyService> logger)
	{
		_privacyPolicyRepository = privacyPolicyRepository;
		_logger = logger;
	}

	public async Task<PrivacyPolicyDto?> GetActivePrivacyPolicy()
	{
		var privacyPolicy = await _privacyPolicyRepository.GetActivePrivacyPolicyAsync();
		
		if (privacyPolicy == null)
			return null;

		AppLogEvents.LogRetrieve(_logger, "active privacy policy", 1);
		return privacyPolicy.ToDTO();
	}

	public async Task<IEnumerable<PrivacyPolicyDto>> GetAllPrivacyPolicies()
	{
		var privacyPolicies = await _privacyPolicyRepository.GetAllPrivacyPoliciesAsync();
		AppLogEvents.LogRetrieve(_logger, "privacy policies", privacyPolicies.Count());
		return privacyPolicies.Select(pp => pp.ToDTO());
	}

	public async Task<PrivacyPolicyDto?> GetPrivacyPolicyById(Guid id)
	{
		var privacyPolicy = await _privacyPolicyRepository.GetPrivacyPolicyByIdAsync(id);
		
		if (privacyPolicy == null)
			return null;

		AppLogEvents.LogRetrieve(_logger, "privacy policy", 1);
		return privacyPolicy.ToDTO();
	}

	public async Task<PrivacyPolicyDto> CreatePrivacyPolicy(CreatePrivacyPolicyDto createDto)
	{
		// Deactivate all existing privacy policies
		await _privacyPolicyRepository.DeactivateAllAsync();

		// Create new privacy policy
		var newPrivacyPolicy = new PrivacyPolicy(
			Guid.NewGuid(),
			createDto.Content,
			DateTime.UtcNow,
			true
		);

		var createdPolicy = await _privacyPolicyRepository.AddAsync(newPrivacyPolicy);
		
		AppLogEvents.LogCreate(_logger, "privacy policy", 1);
		return createdPolicy.ToDTO();
	}
}
