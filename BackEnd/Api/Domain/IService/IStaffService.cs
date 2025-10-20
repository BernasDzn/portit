using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IStaffService
{
	Task<IEnumerable<StaffDto>> GetStaffs();
	Task<StaffDto?> Add(CreateStaffDto staffDto);
	Task<StaffDto?> Update(string mecNumber, CreateStaffDto staffDto);
	Task<Page<StaffDto>> FilterStaffs(StaffFilter filter);
	Task<StaffDto?> Deactivate(string mecanographicNumber);
}