namespace Api.Domain.IRepository;

using Api.Domain.Entities;

public interface ISystemUserRepository : IGenericRepository<SystemUser>
{
    new Task<SystemUser> Add(SystemUser systemUser);
    Task<SystemUser?> GetBySubAsync(string sub);
    Task<SystemUser?> GetByEmailAddressAsync(string emailAddress);
    Task<IEnumerable<SystemUser>> GetAllAsync();
    Task<SystemUser> Update(SystemUser systemUser);
    Task DeleteByEmailAddressAsync(string emailAddress);
    Task DeleteBySubAsync(string sub);
    Task<SystemUser?> GetByActivationTokenAsync(string activationToken);
}