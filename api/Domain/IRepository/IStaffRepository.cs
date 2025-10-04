namespace Domain.IRepository;

using Api.Domain.Model;

public interface IStaffRepository : IGenericRepository<Staff>
{
	Task<IEnumerable<Staff>> GetStaffsAsync();

	Task<Staff?> GetStaffByMecNumberAsync(string mecNumber);

	Task<Page<Staff>> FilterStaffsAsync(StaffFilter filter);

	new Task<Staff> Add(Staff staff);

	Task<Staff> Update(Staff staff);
}