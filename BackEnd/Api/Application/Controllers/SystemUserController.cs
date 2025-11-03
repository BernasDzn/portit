namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;
using System.Linq;
using System.Collections.Generic;

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

    [HttpPut("{sub}/role", Name = "SetUserRole")]
    public async Task<ActionResult<SystemUserDto>> SetUserRole(string sub, int role)
    {
        try
        {
            var updatedUserDto = await _systemUserService.SetUserRole(sub, role);
            return Ok(updatedUserDto);
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with sub '{Sub}' not found for role update", sub);
            return NotFound($"System user with sub '{sub}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error setting role for system user with sub '{Sub}', {Message}", sub, e.Message);
            return StatusCode(500, "An error occurred while setting the user role.");
        }
    }

    [HttpPut("{sub}/deactivate", Name = "DeactivateUser")]
    public async Task<ActionResult> DeactivateUser(string sub)
    {
        try
        {
            var systemUserDto = await _systemUserService.GetBySub(sub);
            systemUserDto.IsActive = false;
            await _systemUserService.UpdateSystemUser(sub, systemUserDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with sub '{Sub}' not found for deactivation", sub);
            return NotFound($"System user with sub '{sub}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error deactivating system user with sub '{Sub}', {Message}", sub, e.Message);
            return StatusCode(500, "An error occurred while deactivating the user.");
        }
    }

    [HttpPut("{sub}/activate", Name = "ActivateUser")]
    public async Task<ActionResult> ActivateUser(string sub)
    {
        try
        {
            var systemUserDto = await _systemUserService.GetBySub(sub);
            systemUserDto.IsActive = true;
            await _systemUserService.UpdateSystemUser(sub, systemUserDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with sub '{Sub}' not found for activation", sub);
            return NotFound($"System user with sub '{sub}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error activating system user with sub '{Sub}', {Message}", sub, e.Message);
            return StatusCode(500, "An error occurred while activating the user.");
        }
    }

    [HttpDelete("{sub}", Name = "DeleteUser")]
    public async Task<ActionResult> DeleteUser(string sub)
    {
        try
        {
            await _systemUserService.DeleteSystemUser(sub);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with sub '{Sub}' not found for deletion", sub);
            return NotFound($"System user with sub '{sub}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error deleting system user with sub '{Sub}', {Message}", sub, e.Message);
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }
}