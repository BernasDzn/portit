using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IStaffService
{
	Task<IEnumerable<StaffDto>> GetStaffs();
	Task<StaffDto?> Create(CreateStaffDto staffDto);
	Task<StaffDto?> Update(string mechanographicNumber, CreateStaffDto staffDto);
	Task<Page<StaffDto>> FilterStaffs(StaffFilter filter);
	Task<StaffDto> Deactivate(string mechanographicNumber);
	Task<int> CountStaffsAsync();
}