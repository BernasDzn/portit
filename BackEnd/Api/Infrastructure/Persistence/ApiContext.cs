namespace Api.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
    }

    // Repositories
    public DbSet<ShippingAgentOrganization> ShippingAgentOrganizations { get; set; } = null!;
    public DbSet<Representative> Representatives { get; set; } = null!;
    public DbSet<Qualification> Qualifications { get; set; } = null!;
    public DbSet<Vessel> Vessels { get; set; } = null!;
    public DbSet<VesselType> VesselTypes { get; set; } = null!;
    public DbSet<Dock> Docks { get; set; } = null!;
    public DbSet<StorageArea> StorageAreas { get; set; } = null!;
    public DbSet<Staff> Staffs { get; set; } = null!;
    public DbSet<PhysicalResource> PhysicalResources { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Discriminator so EF knows to select the correct sub class
        modelBuilder.Entity<PhysicalResource>()
            .HasDiscriminator<string>("resource_type")
            .HasValue<STSCrane>("STSCrane")
            .HasValue<YardCrane>("YardCrane")
            .HasValue<Truck>("Truck");

        // Operation window shift list config
        // Because EF Core does not support collections of owned types directly
        modelBuilder.Entity<OperationalWindow>(ow => {
            ow.OwnsMany(o => o.Shifts, sb => { });
        });
    }
}