using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Api.Application.Services;

public interface ISystemUserService
{
    public Task<SystemUserDto> GetBySub(string sub);
    public Task<IEnumerable<SystemUserDto>> GetAll();
    public Task<SystemUserDto> CreateSystemUser(SystemUserDto systemUserDto);
    public Task<SystemUserDto> UpdateSystemUser(string sub, SystemUserDto systemUserDto);
}