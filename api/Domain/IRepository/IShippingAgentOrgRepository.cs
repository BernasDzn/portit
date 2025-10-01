namespace Domain.IRepository;

using Api.Domain.Model;
using Api.Models;
using Domain.Model.Generic;

public interface IShippingAgentOrgRepository : IGenericRepository<Vessel>
{
    ShippingAgentOrganization? GetByName(string name);
}