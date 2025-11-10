namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer;

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
			IEnumerable<Staff> staffs = await _context.Staffs
				.Where(s => s.isActive).ToListAsync();
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
				.FirstOrDefaultAsync(s => s.MechanographicNumber.Value.Equals(mecNumber) && s.isActive);
			return staff;
		}
		catch (Exception ex)
		{
			throw new EntityNotFoundException("Staff with mechanographic number " + mecNumber + " not found: " + ex.Message);
		}
	}

	public async Task<Staff?> GetStaffByNameAsync(string name)
	{
		try
		{
			Staff? staff = await _context.Staffs
				.FirstOrDefaultAsync(s => s.Name.Equals(name) && s.isActive);
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
				.FirstOrDefaultAsync(s => s.Status.Equals(status) && s.isActive);
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
			query = query.Where(s => s.isActive);

			int pageCount = (int)Math.Ceiling((double)query.Count() / filter.PageSize);

			if (!string.IsNullOrEmpty(filter.MechanographicNumber))
				query = query.Where(s => s.MechanographicNumber.Value.ToLower().Contains(filter.MechanographicNumber.ToLower()));

			if (!string.IsNullOrEmpty(filter.Name))
				query = query.Where(s => s.Name.Value.ToLower().Contains(filter.Name.ToLower()));

			if (!string.IsNullOrEmpty(filter.Email))
				query = query.Where(s => s.Email.Value.ToLower().Contains(filter.Email.ToLower()));

			if (!string.IsNullOrEmpty(filter.PhoneNumber))
				query = query.Where(s => s.PhoneNumber.Value.ToLower().Contains(filter.PhoneNumber.ToLower()));

			if (filter.Status != null)
				query = query.Where(s => s.Status == filter.Status);

			if (filter.QualificationCodes != null && filter.QualificationCodes.Any())
				foreach (string qualificationCode in filter.QualificationCodes)
					query = query.Where(s => s.Qualifications.Any(q => q.NameCode.Value.ToLower().Equals(qualificationCode.ToLower())));

			query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
			return Task.FromResult(
				Page<Staff>.Of(query.ToList(), filter, pageCount)
			);

		}
		catch (Exception ex)
		{
			throw new PersistencyFailedException("Failed to filter staffs: " + ex.Message);
		}
	}
	
	public async Task<int> CountAsync()
	{
		try
		{
			int count = await _context.Staffs
				.Where(s => s.isActive)
				.CountAsync();
			return count;
		}
		catch (Exception ex)
		{
			throw new PersistencyFailedException("Failed to count staffs: " + ex.Message);
		}
	}
}