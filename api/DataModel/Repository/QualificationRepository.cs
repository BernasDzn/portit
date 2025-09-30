using DataModel.Repository;
using Domain.IRepository;
using Api.Domain.Model;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataModel.Repository;

public class QualificationRepository : GenericRepository<Qualification>, IQualificationRepository
{
	public QualificationRepository(ApiContext context) : base(context)
	{

	}

	public async Task<IEnumerable<Qualification>> GetQualificationsAsync()
	{
		try
		{
			IEnumerable<Qualification> qualifications = await _context.Set<Qualification>().ToListAsync();
			return qualifications;
		}
		catch
		{

			throw;
		}
	}

	public async Task<Qualification> GetQualificationByIdAsync(Guid id)
	{
		try
		{
			Qualification? qualification = await _context.Set<Qualification>().FirstOrDefaultAsync(q => q.Id == id);
			return qualification!;
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
			Qualification? qualification = await _context.Set<Qualification>().FirstOrDefaultAsync(q => q.QualificationName == name);
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
			_context.Set<Qualification>().Add(qualification);
			await _context.SaveChangesAsync();
			return qualification;
		}
		catch
		{

			throw;
		}
	}

	public async Task<bool> Update(Qualification qualification, List<string> errorMessage)
	{
		try
		{
			_context.Set<Qualification>().Update(qualification);
			await _context.SaveChangesAsync();
			return true;
		}
		catch (DbUpdateConcurrencyException ex)
		{
			errorMessage.Add("Concurrency error occurred while updating the qualification: " + ex.Message);
			return false;
		}
		catch (Exception ex)
		{
			errorMessage.Add("An error occurred while updating the qualification: " + ex.Message);
			return false;
		}
	}
	
	public async Task<bool> QualificationExists(string name)
	{
		return await _context.Set<Qualification>().AnyAsync(q => q.QualificationName.Equals(name));
	}
	
}