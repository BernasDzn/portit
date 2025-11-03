namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

[ApiController]
[Route("[controller]")]
public class SystemUserController : ControllerBase, ISystemUserController
{
    private readonly ILogger<SystemUserController> _logger;
    private readonly ISystemUserService _systemUserService;

    public SystemUserController(ISystemUserService systemUserService, ILogger<SystemUserController> logger)
    {
        _systemUserService = systemUserService;
        _logger = logger;
    }

    [HttpGet(Name = "GetSystemUsers")]
    public async Task<ActionResult<IEnumerable<SystemUserDto>>> GetAll()
    {
        IEnumerable<SystemUserDto> systemUsersDto = await _systemUserService.GetAll();
        return Ok(systemUsersDto);
    }

    [HttpGet("{sub}", Name = "GetSystemUserBySub")]
    public async Task<ActionResult<SystemUserDto>> GetBySub(string sub)
    {
        try
        {
            var systemUserDto = await _systemUserService.GetBySub(sub);
            return Ok(systemUserDto);
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with sub '{Sub}' not found", sub);
            return NotFound($"System user with sub '{sub}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error retrieving system user with sub '{Sub}', {Message}", sub, e.Message);
            return StatusCode(500, "An error occurred while retrieving the system user.");
        }
    }

    [HttpPost(Name = "CreateSystemUser")]
    public async Task<ActionResult<SystemUserDto>> Create(SystemUserDto systemUserDto)
    {
        try
        {
            var createdUserDto = await _systemUserService.CreateSystemUser(systemUserDto);
            return CreatedAtAction(nameof(GetBySub), new { sub = createdUserDto.Sub }, createdUserDto);
        }
        catch (EntityAlreadyExistsException)
        {
            _logger.LogWarning("System user with sub '{Sub}' already exists", systemUserDto.Sub);
            return Conflict($"System user with sub '{systemUserDto.Sub}' already exists.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error creating system user with sub '{Sub}', {Message}", systemUserDto.Sub, e.Message);
            return StatusCode(500, "An error occurred while creating the system user.");
        }
    }
    [HttpPut("{sub}", Name = "UpdateSystemUser")]
    public async Task<ActionResult<SystemUserDto>> Update(string sub, SystemUserDto systemUserDto)
    {
        try
        {
            var updatedUserDto = await _systemUserService.UpdateSystemUser(sub, systemUserDto);
            return Ok(updatedUserDto);
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with sub '{Sub}' not found for update", sub);
            return NotFound($"System user with sub '{sub}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error updating system user with sub '{Sub}', {Message}", sub, e.Message);
            return StatusCode(500, "An error occurred while updating the system user.");
        }
    }
}