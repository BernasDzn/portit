namespace Api.Domain.IRepository;

using Api.Domain.Entities;

public interface IPrivacyPolicyRepository : IGenericRepository<PrivacyPolicy>
{
	Task<PrivacyPolicy?> GetActivePrivacyPolicyAsync();
	Task<IEnumerable<PrivacyPolicy>> GetAllPrivacyPoliciesAsync();
	Task<PrivacyPolicy?> GetPrivacyPolicyByIdAsync(Guid id);
	Task<PrivacyPolicy> AddAsync(PrivacyPolicy privacyPolicy);
	Task DeactivateAllAsync();
}
