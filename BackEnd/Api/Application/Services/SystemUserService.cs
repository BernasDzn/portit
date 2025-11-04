namespace Api.Application.Services;

using Microsoft.Extensions.Configuration;
using Api.Infrastructure.Utilities.Email;
using System;
using System.Linq;
using System.Threading.Tasks;

using Api.Application.DataTransfer;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

public class SystemUserService : ISystemUserService
{
    private readonly ISystemUserRepository _systemUserRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SystemUserService> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SystemUserService>();

    public SystemUserService(ISystemUserRepository systemUserRepository, IEmailService emailService, IConfiguration configuration, ILogger<SystemUserService> logger)
    {
        _systemUserRepository = systemUserRepository;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<SystemUserDto>> GetAll()
    {
        var systemUsers = await _systemUserRepository.GetAllAsync();
        return systemUsers.Select(su => su.ToDTO());
    }

    public async Task<SystemUserDto> GetBySub(string sub)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with sub '{sub}' not found.");
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }
        return systemUser.ToDTO();
    }

    public async Task<SystemUserDto> CreateSystemUser(SystemUserDto systemUserDto)
    {
        var existingUser = await _systemUserRepository.GetBySubAsync(systemUserDto.Sub);
        if (existingUser != null)
        {
            throw new EntityAlreadyExistsException($"System user with sub '{systemUserDto.Sub}' already exists.");
        }
        // By default newly created users are deactivated
        var systemUser = new SystemUser(systemUserDto.Sub, false, systemUserDto.Email) { Sub = systemUserDto.Sub, Active = false , Email = systemUserDto.Email};
        var createdUser = await _systemUserRepository.Add(systemUser);
        return createdUser.ToDTO();
    }

    public async Task<SystemUserDto> UpdateSystemUser(string sub, SystemUserDto systemUserDto)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with sub '{sub}' not found.");
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }
            
        systemUser.Active = systemUserDto.IsActive;

        var updatedUser = await _systemUserRepository.Update(systemUser);
        return updatedUser.ToDTO();
    }

    public async Task<SystemUserDto> SetUserRole(string sub, int role)
    {
        SystemUser? systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with sub '{sub}' not found.");
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }

        var wasActive = systemUser.Active;

        systemUser.Role = (SystemUserRole)role;

        // If the user is being authorized for the first time (was not active), generate activation token and send email
        if (!wasActive)
        {
            // generate token
            systemUser.ActivationToken = Guid.NewGuid().ToString("N");
            systemUser.ActivationTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            // build activation link based on configuration
            var baseUrl = _configuration.GetValue<string>("ApplicationSettings:BaseUrl") ?? "http://localhost:5173";
            var activationPath = _configuration.GetValue<string>("ApplicationSettings:ActivationPath") ?? "/activate";
            var activationLink = $"{baseUrl.TrimEnd('/')}{activationPath}?token={systemUser.ActivationToken}&sub={systemUser.Sub}";

            // attempt to send the activation email (fire-and-forget not here, but we try)
            try
            {
                var subject = "Activate your account";
                var body = $"Hello,\n\nYou have been authorized to use the system. Please activate your account by clicking the following link:\n\n{activationLink}\n\nThis link expires in 7 days.";
                _logger.LogInformation($"Sending activation email to user with sub '{systemUser.Sub}' at email '{systemUser.Email}'.");
                await _emailService.SendEmailAsync(systemUser.Email, subject, body);
            }
            catch (System.Exception)
            {
                // Log or ignore; don't fail the role assignment because of email issues
                _logger.LogWarning($"Failed to send activation email to user with sub '{systemUser.Sub}'.");
            }
        }

        var updatedUser = await _systemUserRepository.Update(systemUser);
        return updatedUser.ToDTO();
    }

    public async Task DeleteSystemUser(string sub)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with sub '{sub}' not found.");
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }

        await _systemUserRepository.DeleteBySubAsync(sub);
    }

    public async Task ActivateUser(string sub)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with sub '{sub}' not found.");
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }

        if (systemUser.ActivationToken == null)
            throw new InvalidOperationException($"System user with sub '{sub}' does not have a pending activation.");

        systemUser.Active = true;
        systemUser.ActivationToken = null;
        await _systemUserRepository.Update(systemUser);
    }

    public async Task DeactivateUser(string sub)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with sub '{sub}' not found.");
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }

        systemUser.Active = false;
        systemUser.ActivationToken = null;
        await _systemUserRepository.Update(systemUser);
    }
}