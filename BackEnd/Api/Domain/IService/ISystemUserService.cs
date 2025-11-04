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
    public Task<SystemUserDto> SetUserRole(string emailAddress, int role);
    public Task DeleteSystemUser(string emailAddress);
    public Task ActivateUser(string emailAddress);
    public Task DeactivateUser(string emailAddress);
    public Task<SystemUserDto> GetByEmailAddress(string emailAddress);
    public Task<SystemUserDto> ActivateUserWithToken(string emailAddress, string token);
}