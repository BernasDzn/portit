namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase, IAdminController
{
	private readonly ILogger<AdminController> _logger;
    private readonly IAdminService _adminService;

	public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

	[HttpGet("/auditLogs", Name = "AuditLogs")]
	public async Task<ActionResult<IEnumerable<LogDto>>> AuditLogs()
	{
		IEnumerable<LogDto> docks = await _adminService.GetLogs(20);
		return Ok(docks);
	}
}