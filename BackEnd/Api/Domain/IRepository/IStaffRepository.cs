namespace Api.Domain.IRepository;

using Api.Domain.Entities;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IStaffRepository : IGenericRepository<Staff>
{
	Task<IEnumerable<Staff>> GetStaffsAsync();

	Task<int> CountAsync();

	Task<Staff?> GetStaffByMecNumberAsync(string mecNumber);

	Task<Page<Staff>> FilterStaffsAsync(StaffFilter filter);

	new Task<Staff> Add(Staff staff);

	Task<Staff> Update(Staff staff);
}