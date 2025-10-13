using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface IStaffController
{
	public Task<ActionResult<IEnumerable<StaffDto>>> GetAll();
	public Task<ActionResult<StaffDto>> Create(StaffDto staffDto);
	public Task<ActionResult> Deactivate(string mecanographicNumber);
	public Task<ActionResult<StaffDto>> Update(string mecanographicNumber, StaffDto staffDto);
	public Task<ActionResult<Page<StaffDto>>> Filter([FromQuery] StaffFilter filter);
}