using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IAdminService
{
    public Task<IEnumerable<LogDto>> GetLogs(uint max);
}