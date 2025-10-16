namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public class

QualificationRepository : GenericRepository<Qualification>, IQualificationRepository
{
	private new readonly ApiContext _context = null!;

	public QualificationRepository(ApiContext context) : base(context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Qualification>> GetQualificationsAsync()
	{
		try
		{
			IEnumerable<Qualification> qualifications = await _context.Qualifications.ToListAsync();
			return qualifications;
		}
		catch (System.Exception ex)
		{
			throw new PersistencyFailedException("Failed to select qualifications" + ex.Message);
		}
	}

	public async Task<Qualification?> GetQualificationByNameAsync(string name)
	{
		try
		{
			Qualification? qualification = await _context.Qualifications
				.FirstOrDefaultAsync(q => q.QualificationName.Equals(name));
			return qualification;
		}
		catch (System.Exception ex)
		{
			throw new PersistencyFailedException("Failed to select a qualification by name. " + ex.Message);
		}
	}

	public async Task<Qualification?> GetQualificationByIdAsync(string id)
	{
		try
		{
			Qualification? qualification = await _context.Qualifications
				.FirstOrDefaultAsync(q => q.NameCode.Value.Equals(id));
			return qualification;
		}
		catch (System.Exception ex)
		{
			throw new PersistencyFailedException("Failed to select a qualification by id. " + ex.Message);
		}
	}

	public new async Task<Qualification> Add(Qualification qualification)
	{
		try
		{
			_context.Qualifications.Add(qualification);
			await _context.SaveChangesAsync();
			return qualification;
		}
		catch (System.Exception ex)
		{
			throw new PersistencyFailedException("Failed to add a qualification" + ex.Message);
		}
	}

	public async Task<Qualification> Update(Qualification qualification)
	{
		try
		{
			_context.Qualifications.Update(qualification);
			await _context.SaveChangesAsync();
			return qualification;
		}
		catch (System.Exception ex)
		{
			throw new PersistencyFailedException("Failed to update a qualification" + ex.Message);
		}
	}

	public Task<Page<Qualification>> FilterQualificationsAsync(QualificationFilter filter)
	{
		IQueryable<Qualification> query = _context.Qualifications.AsQueryable();
		if (!string.IsNullOrEmpty(filter.Code))
			query = query.Where(q => q.NameCode.Value.ToLower().Contains(filter.Code.ToLower()));

		if (!string.IsNullOrEmpty(filter.QualificationName))
			query = query.Where(q => q.QualificationName.Value.ToLower().Contains(filter.QualificationName.ToLower()));

		query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
		return Task.FromResult(
			Page<Qualification>.Of(query.ToList(), filter)
		);
	}
}