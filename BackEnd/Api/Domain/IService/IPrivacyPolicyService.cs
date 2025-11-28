namespace Api.Domain.IService;

using Api.Application.DataTransfer;

public interface IPrivacyPolicyService
{
	Task<PrivacyPolicyDto?> GetActivePrivacyPolicy();
	Task<IEnumerable<PrivacyPolicyDto>> GetAllPrivacyPolicies();
	Task<PrivacyPolicyDto?> GetPrivacyPolicyById(Guid id);
	Task<PrivacyPolicyDto> CreatePrivacyPolicy(CreatePrivacyPolicyDto createDto);
}
