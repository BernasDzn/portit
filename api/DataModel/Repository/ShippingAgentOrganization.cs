namespace DataModel.Repository;

using Domain.IRepository;
using Api.Domain.Model;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Api.Models;
using Domain.Model.Generic;
using Api.Application.Exceptions;

public class ShippingAgentOrgRepository : GenericRepository<Vessel>, IShippingAgentOrgRepository
{
    private new readonly ApiContext _context = null!;

    public ShippingAgentOrgRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public ShippingAgentOrganization? GetByName(string name)
    {
        try
        {
            ShippingAgentOrganization? shippingAgentOrg = _context.ShippingAgentOrganizations
                .FirstOrDefaultAsync(q => q.LegalName.Value.Equals(name)).Result;
            return shippingAgentOrg;
        }
        catch
        {
            throw new PersistencyFailedException("Failed to select a shipping agent organization by name");
        }
    }
}