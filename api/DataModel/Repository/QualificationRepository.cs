using DataModel.Repository;
using Domain.IRepository;
using Api.Domain.Model;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
			throw;
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
			throw;
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
			throw;
		}
	}

	public async Task<bool> Update(string name, QualificationDto qualificationDto, List<string> errorMessage)
	{
		try
		{
			Qualification? qualification =await GetQualificationByNameAsync(name);
			if (qualification == null)
			{
				errorMessage.Add("Qualification not found.");
				return false;
			}

			qualification.UpdateQualificationName(qualificationDto.QualificationName);

			_context.Qualifications.Update(qualification);
			await _context.SaveChangesAsync();
			return true;
		}
		catch
		{
			throw;
		}
	}	
}