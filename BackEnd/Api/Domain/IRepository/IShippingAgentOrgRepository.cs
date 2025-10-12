namespace Api.Domain.IRepository;

using Api.Domain.Entities;

public interface IShippingAgentOrgRepository : IGenericRepository<ShippingAgentOrganization>
{
    ShippingAgentOrganization? GetByName(string name);
}