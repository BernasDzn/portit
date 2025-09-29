using Api.Domain.Model;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
    }

    // Repositories
    public DbSet<ShippingAgentOrganization> ShippingAgentOrganizations { get; set; } = null!;
    public DbSet<Representative> Representatives { get; set; } = null!;
    public DbSet<Qualification> Qualifications { get; set; } = null!;
}