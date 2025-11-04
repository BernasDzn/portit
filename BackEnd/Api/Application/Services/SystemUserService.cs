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

    public async Task<SystemUserDto> GetByEmailAddress(string emailAddress)
    {
        var systemUser = await _systemUserRepository.GetByEmailAddressAsync(emailAddress);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with email '{emailAddress}' not found.");
            throw new EntityNotFoundException($"System user with email '{emailAddress}' not found.");
        }
        return systemUser.ToDTO();
    }

    public async Task<SystemUserDto> CreateSystemUser(SystemUserDto systemUserDto)
    {
        var existingUser = await _systemUserRepository.GetByEmailAddressAsync(systemUserDto.Email);
        if (existingUser != null)
        {
            throw new EntityAlreadyExistsException($"System user with email '{systemUserDto.Email}' already exists.");
        }
        // By default newly created users are deactivated
        var systemUser = new SystemUser(null, false, systemUserDto.Email) { Sub = null, Active = false , Email = systemUserDto.Email};
        var createdUser = await _systemUserRepository.Add(systemUser);
        return createdUser.ToDTO();
    }

    public async Task<SystemUserDto> UpdateSystemUser(string emailAddress, SystemUserDto systemUserDto)
    {
        var systemUser = await _systemUserRepository.GetByEmailAddressAsync(emailAddress);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with email '{emailAddress}' not found.");
            throw new EntityNotFoundException($"System user with email '{emailAddress}' not found.");
        }
            
        systemUser.Active = systemUserDto.IsActive ?? systemUser.Active;

        var updatedUser = await _systemUserRepository.Update(systemUser);
        return updatedUser.ToDTO();
    }

    public async Task<SystemUserDto> SetUserRole(string emailAddress, int role)
    {
        SystemUser? systemUser = await _systemUserRepository.GetByEmailAddressAsync(emailAddress);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with email address '{emailAddress}' not found.");
            throw new EntityNotFoundException($"System user with email address '{emailAddress}' not found.");
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
            var activationPath = _configuration.GetValue<string>("ApplicationSettings:ActivationPath") ?? "/activate-with-token";
            var activationLink = $"{baseUrl.TrimEnd('/')}{activationPath}?token={systemUser.ActivationToken}&emailAddress={systemUser.Email}";

            // attempt to send the activation email (fire-and-forget not here, but we try)
            try
            {
                var subject = "Activate your account";
                var body = $"Hello,\n\nYou have been authorized to use the system. Please activate your account by clicking the following link:\n\n{activationLink}\n\nThis link expires in 7 days.";
                _logger.LogInformation($"Sending activation email to email '{systemUser.Email}'.");
                await _emailService.SendEmailAsync(systemUser.Email!, subject, body);
            }
            catch (System.Exception)
            {
                // Log or ignore; don't fail the role assignment because of email issues
                _logger.LogWarning($"Failed to send activation email to user '{systemUser.Email}'.");
            }
        }

        var updatedUser = await _systemUserRepository.Update(systemUser);
        return updatedUser.ToDTO();
    }
    public async Task<SystemUserDto> ActivateUserWithToken(string emailAddress, string token, string sub)
    {
        var systemUser = await _systemUserRepository.GetByActivationTokenAsync(token);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with email '{emailAddress}' not found.");
            throw new EntityNotFoundException($"System user with email '{emailAddress}' not found.");
        }

        if (systemUser.ActivationToken != token || systemUser.ActivationTokenExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning($"Invalid or expired activation token for user '{emailAddress}'.");
            throw new InvalidOperationException($"Invalid or expired activation token for user '{emailAddress}'.");
        }

        if(systemUser.Email != emailAddress)
        {
            _logger.LogWarning($"Activation token does not match email address '{emailAddress}'.");
            throw new InvalidOperationException($"Activation token does not match email address '{emailAddress}'.");
        }

        systemUser.Active = true;
        systemUser.ActivationToken = null;
        systemUser.ActivationTokenExpiresAt = null;
        systemUser.Sub = sub;

        var updatedUser = await _systemUserRepository.Update(systemUser);
        return updatedUser.ToDTO();
    }

    public async Task DeleteSystemUser(string emailAddress)
    {
        var systemUser = await _systemUserRepository.GetByEmailAddressAsync(emailAddress);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with email '{emailAddress}' not found.");
            throw new EntityNotFoundException($"System user with email '{emailAddress}' not found.");
        }

        await _systemUserRepository.DeleteByEmailAddressAsync(emailAddress);
    }

    public async Task ActivateUser(string emailAddress)
    {
        var systemUser = await _systemUserRepository.GetByEmailAddressAsync(emailAddress);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with email '{emailAddress}' not found.");
            throw new EntityNotFoundException($"System user with email '{emailAddress}' not found.");
        }

        if (systemUser.ActivationToken == null)
            throw new InvalidOperationException($"System user with email '{emailAddress}' does not have a pending activation.");

        systemUser.Active = true;
        systemUser.ActivationToken = null;
        await _systemUserRepository.Update(systemUser);
    }

    public async Task DeactivateUser(string emailAddress)
    {
        var systemUser = await _systemUserRepository.GetByEmailAddressAsync(emailAddress);
        if (systemUser == null)
        {
            _logger.LogWarning($"System user with email '{emailAddress}' not found.");
            throw new EntityNotFoundException($"System user with email '{emailAddress}' not found.");
        }

        systemUser.Active = false;
        systemUser.ActivationToken = null;
        await _systemUserRepository.Update(systemUser);
    }
}