namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

public class SystemUserService : ISystemUserService
{
    private readonly ISystemUserRepository _systemUserRepository;

    public SystemUserService(ISystemUserRepository systemUserRepository)
    {
        _systemUserRepository = systemUserRepository;
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

        var systemUser = new SystemUser(systemUserDto.Sub, systemUserDto.IsActive) { Sub = systemUserDto.Sub, Active = systemUserDto.IsActive };
        var createdUser = await _systemUserRepository.Add(systemUser);
        return createdUser.ToDTO();
    }

    public async Task<SystemUserDto> UpdateSystemUser(string sub, SystemUserDto systemUserDto)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }

        systemUser.Active = systemUserDto.IsActive;
        var updatedUser = await _systemUserRepository.Update(systemUser);
        return updatedUser.ToDTO();
    }

    public async Task<SystemUserDto> SetUserRole(string sub, int role)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }

        systemUser.Role = (SystemUserRole)role;
        var updatedUser = await _systemUserRepository.Update(systemUser);
        return updatedUser.ToDTO();
    }

    public async Task DeleteSystemUser(string sub)
    {
        var systemUser = await _systemUserRepository.GetBySubAsync(sub);
        if (systemUser == null)
        {
            throw new EntityNotFoundException($"System user with sub '{sub}' not found.");
        }

        await _systemUserRepository.DeleteBySubAsync(sub);
    }
}