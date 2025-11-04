using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface ISystemUserController
{
    public Task<ActionResult<SystemUserDto>> GetByEmailAddress(string emailAddress);
    public Task<ActionResult<IEnumerable<SystemUserDto>>> GetAll();
    public Task<ActionResult<SystemUserDto>> Create(SystemUserDto systemUserDto);
    public Task<ActionResult<SystemUserDto>> SetUserRole(string emailAddress, int role);
    public Task<ActionResult> DeactivateUser(string emailAddress);
    public Task<ActionResult> ActivateUser(string emailAddress);
    public Task<ActionResult> DeleteUser(string emailAddress);
    public Task<ActionResult> ActivateUserWithToken(string emailAddress, string token, ActivationIdTokenRequest idTokenRequest);
    public Task<ActionResult<Page<SystemUserDto>>> FilterUsers(SystemUserFilter filter);
}