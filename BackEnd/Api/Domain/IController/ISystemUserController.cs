using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;

public interface ISystemUserController
{
    public Task<ActionResult<SystemUserDto>> GetBySub(string sub);
    public Task<ActionResult<IEnumerable<SystemUserDto>>> GetAll();
    public Task<ActionResult<SystemUserDto>> Create(SystemUserDto systemUserDto);
    public Task<ActionResult<SystemUserDto>> SetUserRole(string sub, int role);
    public Task<ActionResult> DeactivateUser(string sub);
    public Task<ActionResult> ActivateUser(string sub);
    public Task<ActionResult> DeleteUser(string sub);
}