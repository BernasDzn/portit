namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;

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
}