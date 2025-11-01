using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface IStaffController
{
	public Task<ActionResult<IEnumerable<StaffDto>>> GetAll();
	public Task<ActionResult<StaffDto>> Create(CreateStaffDto staffDto);
	public Task<ActionResult> Deactivate(string mechanographicNumber);
	public Task<ActionResult<StaffDto>> Update(string mechanographicNumber, CreateStaffDto staffDto);
	public Task<ActionResult<Page<StaffDto>>> Filter([FromQuery] StaffFilter filter);
}