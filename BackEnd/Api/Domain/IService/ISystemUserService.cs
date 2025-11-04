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
    public Task<SystemUserDto> SetUserRole(string sub, int role);
    public Task DeleteSystemUser(string sub);
    public Task ActivateUser(string sub);
    public Task DeactivateUser(string sub);
}