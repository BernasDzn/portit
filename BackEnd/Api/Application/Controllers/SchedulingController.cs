namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;
using System.Net;

[ApiController]
[Route("[controller]")]
public class SchedulingController : ControllerBase, ISchedulingController
{
	private readonly ILogger<QualificationController> _logger;

	public SchedulingController(ILogger<QualificationController> logger)
	{
		_logger = logger;
	}

	[HttpGet(Name = "ScheduleOnDay")]
	public async Task<ActionResult<IEnumerable<QualificationDto>>> Schedule()
	{
		try
		{
            // Get from prolog server
            WebClient client = new HttpWebRequest();
            string response = await client.DownloadStringTaskAsync("http://localhost:8080/schedule");

			return Ok();
		}
		catch (System.Exception)
		{
			_logger.LogCritical("Error scheduling on day.");
			return StatusCode(500, "An error occurred while scheduling on day.");
		}
	}
}