using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
namespace Api.Application.Services;

public interface IStaffService
{
	Task<IEnumerable<StaffDto>> GetStaffs();
	Task<StaffDto?> Add(StaffDto staffDto);
	Task<StaffDto?> Update(string mecNumber, StaffDto staffDto);
	Task<Page<StaffDto>> FilterStaffs(StaffFilter filter);
	Task<StaffDto?> Deactivate(string mecanographicNumber);
}