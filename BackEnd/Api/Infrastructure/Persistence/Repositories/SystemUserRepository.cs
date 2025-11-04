namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public class SystemUserRepository : GenericRepository<SystemUser>, ISystemUserRepository
{
    private new readonly ApiContext _context = null!;

    public SystemUserRepository(ApiContext context) : base(context)
    {
        _context = context;
    }


    public async Task<IEnumerable<SystemUser>> GetAllAsync()
    {
        try
        {
            IEnumerable<SystemUser> systemUsers = await _context.SystemUsers.ToListAsync();
            return systemUsers;
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to select all system users. " + ex.Message);
        }
    }

    public async Task<SystemUser?> GetBySubAsync(string sub)
    {
        try
        {
            // Use the mapped property `Sub` and the == operator so EF Core can translate the expression
            SystemUser? systemUser = await _context.SystemUsers
                .FirstOrDefaultAsync(su => su.Sub == sub);
            return systemUser;
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to select a system user by sub. " + ex.Message);
        }
    }

    public new async Task<SystemUser> Add(SystemUser systemUser)
    {
        try
        {
            await _context.SystemUsers.AddAsync(systemUser);
            await _context.SaveChangesAsync();
            return systemUser;
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to insert a system user. " + ex.Message);
        }
    }

    public async Task<SystemUser> Update(SystemUser systemUser)
    {
        try
        {
            _context.SystemUsers.Update(systemUser);
            await _context.SaveChangesAsync();
            return systemUser;
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to update a system user. " + ex.Message);
        }
    }

    public async Task DeleteBySubAsync(string sub)
    {
        try
        {
            var systemUser = await GetBySubAsync(sub);
            if (systemUser != null)
            {
                _context.SystemUsers.Remove(systemUser);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to delete a system user by sub. " + ex.Message);
        }
    }

    public async Task DeleteByEmailAddressAsync(string emailAddress)
    {
        try
        {
            var systemUser = await _context.SystemUsers
                .FirstOrDefaultAsync(su => su.Email == emailAddress);
            if (systemUser != null)
            {
                _context.SystemUsers.Remove(systemUser);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to delete a system user by email address. " + ex.Message);
        }
    }

    public async Task<SystemUser?> GetByActivationTokenAsync(string activationToken)
    {
        try
        {
            SystemUser? systemUser = await _context.SystemUsers
                .FirstOrDefaultAsync(su => su.ActivationToken == activationToken);
            return systemUser;
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to select a system user by authentication token. " + ex.Message);
        }
    }

    public async Task<SystemUser?> GetByEmailAddressAsync(string emailAddress)
    {
        try
        {
            SystemUser? systemUser = await _context.SystemUsers
                .FirstOrDefaultAsync(su => su.Email == emailAddress);
            return systemUser;
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to select a system user by email address. " + ex.Message);
        }
    }

    public async Task<Page<SystemUser>> FilterUsersAsync(SystemUserFilter filter)
    {
        try
        {
            IQueryable<SystemUser> query = _context.SystemUsers.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(su => su.Email!.Contains(filter.Email));
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(su => su.Active == filter.IsActive.Value);
            }

            if (filter.Role.HasValue)
            {
                query = query.Where(su => su.Role == (SystemUserRole)filter.Role.Value);
            }

            int pageCount = (int) Math.Ceiling((double)query.Count() / filter.PageSize);
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            return Page<SystemUser>.Of(await query.ToListAsync(), filter, pageCount);
        }
        catch (Exception ex)
        {
            throw new PersistencyFailedException("Failed to filter system users. " + ex.Message);
        }
    }
}