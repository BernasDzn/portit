namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

public class PrivacyPolicyRepository : GenericRepository<PrivacyPolicy>, IPrivacyPolicyRepository
{
	private new readonly ApiContext _context;

	public PrivacyPolicyRepository(ApiContext context) : base(context)
	{
		_context = context;
	}

	public async Task<PrivacyPolicy?> GetActivePrivacyPolicyAsync()
	{
		try
		{
			return await _context.PrivacyPolicies
				.Where(pp => pp.Active)
				.OrderByDescending(pp => pp.UpdatedOn)
				.FirstOrDefaultAsync();
		}
		catch (Exception)
		{
			throw new PersistencyFailedException("Failed to retrieve active privacy policy from the database.");
		}
	}

	public async Task<IEnumerable<PrivacyPolicy>> GetAllPrivacyPoliciesAsync()
	{
		try
		{
			return await _context.PrivacyPolicies
				.OrderByDescending(pp => pp.UpdatedOn)
				.ToListAsync();
		}
		catch (Exception)
		{
			throw new PersistencyFailedException("Failed to retrieve privacy policies from the database.");
		}
	}

	public async Task<PrivacyPolicy?> GetPrivacyPolicyByIdAsync(Guid id)
	{
		try
		{
			return await _context.PrivacyPolicies
				.FirstOrDefaultAsync(pp => pp.Id == id);
		}
		catch (Exception)
		{
			throw new PersistencyFailedException("Failed to retrieve privacy policy from the database.");
		}
	}

	public async Task<PrivacyPolicy> AddAsync(PrivacyPolicy privacyPolicy)
	{
		try
		{
			_context.PrivacyPolicies.Add(privacyPolicy);
			await _context.SaveChangesAsync();
			return privacyPolicy;
		}
		catch (Exception)
		{
			throw new PersistencyFailedException("Failed to add privacy policy to the database.");
		}
	}

	public async Task DeactivateAllAsync()
	{
		try
		{
			var activePolicies = await _context.PrivacyPolicies
				.Where(pp => pp.Active)
				.ToListAsync();

			foreach (var policy in activePolicies)
			{
				policy.Deactivate();
			}

			await _context.SaveChangesAsync();
		}
		catch (Exception)
		{
			throw new PersistencyFailedException("Failed to deactivate privacy policies in the database.");
		}
	}
}
