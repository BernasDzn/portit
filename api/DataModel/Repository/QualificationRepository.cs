using DataModel.Repository;
using Domain.IRepository;
using Api.Domain.Model;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Api.Application.Exceptions;

namespace DataModel.Repository;

public class QualificationRepository : GenericRepository<Qualification>, IQualificationRepository
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
		catch
		{
			throw new PersistencyFailedException("Failed to select qualifications");
		}
	}

	public async Task<Qualification> GetQualificationByNameAsync(string name)
	{
		try
		{
			Qualification? qualification = await _context.Qualifications
				.FirstOrDefaultAsync(q => q.QualificationName.Value.Equals(name));
			return qualification!;
		}
		catch
		{
			throw new PersistencyFailedException("Failed to select a qualification by name");
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
		catch
		{
			throw new PersistencyFailedException("Failed to add a qualification");
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
		catch
		{
			throw new PersistencyFailedException("Failed to update a qualification");
		}
	}
}