namespace Api.Domain.IRepository;

using System.Threading.Tasks;
using Api.Domain.Entities;

public interface IShippingAgentOrgRepository : IGenericRepository<ShippingAgentOrganization>
{
    ShippingAgentOrganization? GetByName(string name);
    Task<ShippingAgentOrganization?> GetByRepEmailAsync(string ownerEmail);
}