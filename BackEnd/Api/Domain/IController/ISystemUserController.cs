using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;

public interface ISystemUserController
{
    public Task<ActionResult<SystemUserDto>> GetBySub(string sub);
    public Task<ActionResult<IEnumerable<SystemUserDto>>> GetAll();
    public Task<ActionResult<SystemUserDto>> Create(SystemUserDto systemUserDto);
    public Task<ActionResult<SystemUserDto>> SetUserRole(string emailAddress, int role);
    public Task<ActionResult> DeactivateUser(string emailAddress);
    public Task<ActionResult> ActivateUser(string emailAddress);
    public Task<ActionResult> DeleteUser(string emailAddress);
}