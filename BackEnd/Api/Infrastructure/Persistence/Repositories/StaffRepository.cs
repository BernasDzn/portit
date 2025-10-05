namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public class StaffRepository : GenericRepository<Staff>, IStaffRepository
{
	private new readonly ApiContext _context = null!;

	public StaffRepository(ApiContext context) : base(context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Staff>> GetStaffsAsync()
	{
		try
		{
			IEnumerable<Staff> staffs = await _context.Staffs.ToListAsync();
			return staffs;
		}
		catch (Exception ex)
		{
			throw new PersistencyFailedException("Failed to select staffs: " + ex.Message);
		}
	}

	public async Task<Staff?> GetStaffByMecNumberAsync(string mecNumber)
	{
		try
		{
			Staff? staff = await _context.Staffs
				.FirstOrDefaultAsync(s => s.MechanograficNumber.Value.Equals(mecNumber));
			return staff;
		}
		catch (Exception ex)
		{
			throw new EntityNotFoundException("Staff with mechanografic number " + mecNumber + " not found: " + ex.Message);
		}
	}

	public async Task<Staff?> GetStaffByNameAsync(string name)
	{
		try
		{
			Staff? staff = await _context.Staffs
				.FirstOrDefaultAsync(s => s.Name.Equals(name));
			return staff;
		}
		catch (Exception ex)
		{
			throw new EntityNotFoundException("Staff with name " + name + " not found: " + ex.Message);
		}
	}

	public async Task<Staff?> GetStaffByStatusAsync(StaffStatus status)
	{
		try
		{
			Staff? staff = await _context.Staffs
				.FirstOrDefaultAsync(s => s.Status.Equals(status));
			return staff;
		}
		catch (Exception ex)
		{
			throw new EntityNotFoundException("Staff with status " + status + " not found: " + ex.Message);
		}
	}

	public new async Task<Staff> Add(Staff staff)
	{
		try
		{
			_context.Staffs.Add(staff);
			await _context.SaveChangesAsync();
			return staff;
		}
		catch (Exception ex)
		{
			throw new PersistencyFailedException("Failed to add a staff: " + ex.Message);
		}
	}

	public async Task<Staff> Update(Staff staff)
	{
		try
		{
			_context.Staffs.Update(staff);
			await _context.SaveChangesAsync();
			return staff;
		}
		catch (Exception ex)
		{
			throw new PersistencyFailedException("Failed to update a staff: " + ex.Message);
		}
	}

	public Task<Page<Staff>> FilterStaffsAsync(StaffFilter filter)
	{
		try
		{
			IQueryable<Staff> query = _context.Staffs.AsQueryable();

			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(s => s.Name.Value.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
			}

			if (!string.IsNullOrEmpty(filter.Status))
			{
				query = query.Where(s => s.Status.ToString().Equals(filter.Status, StringComparison.OrdinalIgnoreCase));
			}

			if (filter.Qualifications != null && filter.Qualifications.Any())
			{
				query = query.Include(s => s.Qualifications);
				foreach (var qualification in filter.Qualifications)
				{
					query = query.Where(s => s.Qualifications.Any(q => q.QualificationName.Value.Equals(qualification.QualificationName, StringComparison.OrdinalIgnoreCase)));
				}
			}
			
			query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
			return Task.FromResult(
				Page<Staff>.Of(query.ToList(), filter)
			);
			
		}
		catch (Exception ex)
		{
			throw new PersistencyFailedException("Failed to filter staffs: " + ex.Message);
		}
	}
	


	
}