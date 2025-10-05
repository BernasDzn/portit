namespace Api.Domain.IRepository;

using Api.Domain.Entities;

public interface IShippingAgentOrgRepository : IGenericRepository<Vessel>
{
    ShippingAgentOrganization? GetByName(string name);
}