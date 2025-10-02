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
}