using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;

public interface ISystemUserController
{
    public Task<ActionResult<SystemUserDto>> GetBySub(string sub);
    public Task<ActionResult<IEnumerable<SystemUserDto>>> GetAll();
    public Task<ActionResult<SystemUserDto>> Create(SystemUserDto systemUserDto);
    public Task<ActionResult<SystemUserDto>> Update(string sub, SystemUserDto systemUserDto);
}