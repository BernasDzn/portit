namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;

public class ShippingAgentOrgRepository : GenericRepository<ShippingAgentOrganization>, IShippingAgentOrgRepository
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