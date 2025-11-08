namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Google.Apis.Auth;
using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;
using System.Linq;
using System.Collections.Generic;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "ApiUser")]
public class SystemUserController : ControllerBase, ISystemUserController
{
    private readonly ILogger<SystemUserController> _logger;
    private readonly ISystemUserService _systemUserService;
    private readonly IConfiguration _configuration;

    public SystemUserController(ISystemUserService systemUserService, ILogger<SystemUserController> logger, IConfiguration configuration)
    {
        _systemUserService = systemUserService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet(Name = "GetSystemUsers")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IEnumerable<SystemUserDto>>> GetAll()
    {
        IEnumerable<SystemUserDto> systemUsersDto = await _systemUserService.GetAll();
        return Ok(systemUsersDto);
    }

    [HttpGet("{emailAddress}", Name = "GetSystemUserByEmailAddress")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<SystemUserDto>> GetByEmailAddress(string emailAddress)
    {
        try
        {
            var systemUserDto = await _systemUserService.GetByEmailAddress(emailAddress);
            return Ok(systemUserDto);
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with email '{emailAddress}' not found", emailAddress);
            return NotFound($"System user with email '{emailAddress}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error retrieving system user with email '{emailAddress}', {Message}", emailAddress, e.Message);
            return StatusCode(500, "An error occurred while retrieving the system user.");
        }
    }

    [HttpPost(Name = "CreateSystemUser")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<SystemUserDto>> Create(SystemUserDto systemUserDto)
    {
        try
        {
            var createdUserDto = await _systemUserService.CreateSystemUser(systemUserDto);
            return CreatedAtAction(nameof(Create), new { email = createdUserDto.Email }, createdUserDto);
        }
        catch (EntityAlreadyExistsException)
        {
            _logger.LogWarning("System user with email '{email}' already exists.", systemUserDto.Email);
            return Conflict($"System user with email '{systemUserDto.Email}' already exists.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error creating system user with email '{email}', {Message}", systemUserDto.Email, e.Message);
            return StatusCode(500, "An error occurred while creating the system user.");
        }
    }

    [HttpPut("{emailAddress}/role", Name = "SetUserRole")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<SystemUserDto>> SetUserRole(string emailAddress, int role)
    {
        try
        {
            var updatedUserDto = await _systemUserService.SetUserRole(emailAddress, role);
            return Ok(updatedUserDto);
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with email '{Email}' not found for role update", emailAddress);
            return NotFound($"System user with email '{emailAddress}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error setting role for system user with email '{Email}', {Message}", emailAddress, e.Message);
            return StatusCode(500, "An error occurred while setting the user role.");
        }
    }

    [HttpPut("{emailAddress}/deactivate", Name = "DeactivateUser")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> DeactivateUser(string emailAddress)
    {
        try
        {
            await _systemUserService.DeactivateUser(emailAddress);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with email '{email}' not found for deactivation", emailAddress);
            return NotFound($"System user with email '{emailAddress}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error deactivating system user with email '{email}', {Message}", emailAddress, e.Message);
            return StatusCode(500, "An error occurred while deactivating the user.");
        }
    }

    [HttpPut("{emailAddress}/activate", Name = "ActivateUser")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> ActivateUser(string emailAddress)
    {
        try
        {
            await _systemUserService.ActivateUser(emailAddress);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with email '{email}' not found for activation", emailAddress);
            return NotFound($"System user with email '{emailAddress}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error activating system user with email '{email}', {Message}", emailAddress, e.Message);
            return StatusCode(500, "An error occurred while activating the user.");
        }
    }

    [HttpPost("activate-with-token", Name = "ActivateUserWithToken")]
    [AllowAnonymous]
    public async Task<ActionResult> ActivateUserWithToken(string emailAddress, string token, [FromBody] Api.Application.DataTransfer.ActivationIdTokenRequest idTokenRequest)
    {
        try
        {
            if (idTokenRequest == null || string.IsNullOrEmpty(idTokenRequest.IdToken))
                return BadRequest("Missing id_token in request body.");

            // Validate the Google ID token server-side to prevent trusting client-side values
            var audience = _configuration.GetValue<string>("Authentication:Google:ClientId");
            var settings = new GoogleJsonWebSignature.ValidationSettings();
            if (!string.IsNullOrEmpty(audience))
            {
                settings.Audience = new[] { audience };
            }
            var payload = await GoogleJsonWebSignature.ValidateAsync(idTokenRequest.IdToken, settings);

            var sub = payload.Subject;

            // Optional: verify payload.Email matches the emailAddress query parameter
            if (!string.Equals(payload.Email ?? string.Empty, emailAddress, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Email in ID token '{TokenEmail}' does not match activation email '{Email}'", payload.Email, emailAddress);
                return BadRequest("Email in ID token does not match activation email.");
            }

            await _systemUserService.ActivateUserWithToken(emailAddress, token, sub);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with email '{email}' not found for activation with token", emailAddress);
            return NotFound($"System user with email '{emailAddress}' not found.");
        }
        catch (InvalidJwtException ij)
        {
            _logger.LogWarning("Invalid Google ID token during activation for '{Email}': {Msg}", emailAddress, ij.Message);
            return BadRequest("Invalid Google ID token.");
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("Invalid or expired activation token for user '{email}'", emailAddress);
            return BadRequest($"Invalid or expired activation token for user '{emailAddress}'.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error activating system user with email '{email}' using token, {Message}", emailAddress, e.Message);
            return StatusCode(500, "An error occurred while activating the user with token.");
        }
    }

    [HttpDelete("{emailAddress}", Name = "DeleteUser")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> DeleteUser(string emailAddress)
    {
        try
        {
            await _systemUserService.DeleteSystemUser(emailAddress);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("System user with email '{email}' not found for deletion", emailAddress);
            return NotFound($"System user with email '{emailAddress}' not found.");
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error deleting system user with email '{email}', {Message}", emailAddress, e.Message);
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }

    [HttpGet("filter")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<Page<SystemUserDto>>> FilterUsers([FromQuery]SystemUserFilter filter)
    {
        try
        {
            var filteredUsers = await _systemUserService.FilterUsers(filter);
            return Ok(filteredUsers);
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error filtering system users, {Message}", e.Message);
            return StatusCode(500, "An error occurred while filtering the users.");
        }
    }
}