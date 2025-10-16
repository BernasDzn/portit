namespace Api.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using System.Text.Json;
using Namotion.Reflection;

public class ApiContext : DbContext
{

    protected readonly IConfiguration Configuration;

    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
        Database.EnsureCreated();
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
    public DbSet<VesselVisitNotification> VesselVisitNotifications { get; set; } = null!;
    public DbSet<Container> Containers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Discriminator so EF knows to select the correct sub class
        // modelBuilder.Entity<PhysicalResource>()
        //     .HasDiscriminator<string>("resource_type")
        //     .HasValue<STSCrane>("STSCrane")
        //     .HasValue<YardCrane>("YardCrane")
        //     .HasValue<Truck>("Truck");

        modelBuilder.Entity<PhysicalResource>(entity =>
        {
            entity.HasDiscriminator<string>("resource_type")
                .HasValue<STSCrane>("STSCrane")
                .HasValue<YardCrane>("YardCrane")
                .HasValue<Truck>("Truck");

            entity.Property(e => e.OperationalWindow)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<OperationalWindow>(v, (JsonSerializerOptions?)null)!
            )
            .HasColumnType("json");
        });

        // Operation window shift list config
        // Because EF Core does not support collections of owned types directly
        // Crew safety officers list config
        // Because EF Core does not support collections of owned types directly

        // Notification decisions list config
        // Because EF Core does not support collections of owned types directly
        /* modelBuilder.Entity<VesselVisitNotification>(v =>
         {
             v.OwnsMany(vn => vn.NotificationDecisions, nd => { });
         });*/

        modelBuilder.Entity<Staff>()
            .Property(e => e.OperationalWindow)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<OperationalWindow>(v, (JsonSerializerOptions?)null)!
            )
            .HasColumnType("json");

        modelBuilder.Entity<StorageArea>(entity =>
        {
            entity.OwnsMany(e => e.DockServices, dr =>
            {
                dr.WithOwner().HasForeignKey("StorageAreaId");

                dr.Property<int>("Id");
                dr.HasKey("Id");
                dr.Property(d => d.IsServingDock);
                dr.Property(d => d.Distance);
            });
        });

        // Encryption for sensitive data
        modelBuilder.Entity<Representative>(entity =>
        {
            entity.Property(e => e.CitizenshipId).HasConversion<UIntEncryptionConvertor>();
            entity.Property(e => e.EmailAddress).HasConversion<EmailEncryptionConverter>();
            entity.Property(e => e.Phone).HasConversion<PhoneNumberEncryptionConverter>();
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(e => e.Email).HasConversion<EmailEncryptionConverter>();
            entity.Property(e => e.PhoneNumber).HasConversion<PhoneNumberEncryptionConverter>();
        });

        modelBuilder.Entity<ShippingAgentOrganization>(entity =>
        {
            entity.OwnsMany(e => e.AltNames, an =>
            {
                an.WithOwner().HasForeignKey("ShippingAgentOrganizationId");
                an.Property<int>("Id");
                an.HasKey("Id");
                an.Property(a => a.Value);
            });
        });

        modelBuilder.Entity<VesselVisitNotification>(entity =>
        {
            // entity.Property(e => e.CrewDetails).HasConversion(
            //     v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            //     v => JsonSerializer.Deserialize<Crew>(v, (JsonSerializerOptions?)null)
            // );
            entity.OwnsOne(e => e.CrewDetails, cd =>
            {
                cd.Property(c => c.TotalCrewMembers).HasColumnName("TotalCrewMembers");
                cd.OwnsOne(c => c.Captain, cap =>
                {
                    cap.Property(c => c.Value).HasColumnName("Captain");
                });

                // Store the safety officers only as a JSON field
                cd.Property(c => c.SafetyOfficers)
                  .HasColumnName("SafetyOfficers")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<ICollection<SafetyOfficer>>(v, (JsonSerializerOptions?)null)
                  )
                  .HasColumnType("json");
            });

            // LoadCargoManifest
            entity.OwnsMany(e => e.LoadCargoManifest, cm =>
            {
                cm.WithOwner().HasForeignKey("VesselVisitNotificationId");

                cm.HasKey("VesselVisitNotificationId", "ContainerId");
                cm.OwnsOne(c => c.Position, pos =>
                {
                    pos.Property(p => p.Bay).HasColumnName("Position_Bay");
                    pos.Property(p => p.Row).HasColumnName("Position_Row");
                    pos.Property(p => p.Tier).HasColumnName("Position_Tier");
                });

                cm.HasOne(c => c.Area)
                  .WithMany()
                  .HasForeignKey("StorageAreaId")
                  .OnDelete(DeleteBehavior.Restrict);

                cm.HasOne(c => c.Container)
                    .WithMany()
                    .HasForeignKey("ContainerId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UnloadCargoManifest
            entity.OwnsMany(e => e.UnloadCargoManifest, cm =>
            {
                cm.WithOwner().HasForeignKey("VesselVisitNotificationId");
                cm.HasKey("VesselVisitNotificationId", "ContainerId");

                cm.OwnsOne(c => c.Position, pos =>
                {
                    pos.Property(p => p.Bay).HasColumnName("Position_Bay");
                    pos.Property(p => p.Row).HasColumnName("Position_Row");
                    pos.Property(p => p.Tier).HasColumnName("Position_Tier");
                });

                cm.HasOne(c => c.Area)
                  .WithMany()
                  .HasForeignKey("StorageAreaId")
                  .OnDelete(DeleteBehavior.Restrict);

                cm.HasOne(c => c.Container)
                    .WithMany()
                    .HasForeignKey("ContainerId")
                    .OnDelete(DeleteBehavior.Restrict);
            });
        });
    }
}