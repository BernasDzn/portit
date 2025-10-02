using Api.Domain.Model;
using Api.Models;
using Domain;
using Domain.Model.Generic;

namespace DAL;

public static class Bootstrap
{

    public static void Init(ApiContext context, bool nukeDatabase = false)
    {
        // Delete the database if we need that
        if (nukeDatabase)
            context.Database.EnsureDeleted();

        // Ensure the database is created
        context.Database.EnsureCreated();

        // Bootstrap Qualifications
        BootstrapQualifications(context);
        // Bootstrap Shipping Agent Organizations and Representatives
        BootstrapSAO(context);
        // Bootstrap Vessels, Vessel Types and docks
        BootstrapVesselsAndDocks(context);
        // Bootstrap Storage Areas
        BootstrapStorageAreas(context);
    }

    private static void BootstrapQualifications(ApiContext context)
    {
        // Check if there are any qualifications already in the database
        if (context.Qualifications.Any())
            return;

        // Add Bootstrap data
        context.Qualifications.AddRange(
            new Qualification(Guid.NewGuid(), new Code { Value = "STSOP" }, new Designation { Value = " STS Crane Operator" }),
            new Qualification(Guid.NewGuid(), new Code { Value = "YACOP" }, new Designation { Value = "Yard Crane Operator" }),
            new Qualification(Guid.NewGuid(), new Code { Value = "TRKDR" }, new Designation { Value = "Truck Driver" })
        );

        context.SaveChanges();
    }

    private static void BootstrapSAO(ApiContext context)
    {
        // Check if there are any representatives already in the database
        if (context.ShippingAgentOrganizations.Any())
            return;

        Representative r = new Representative(Guid.NewGuid(), 908029952, new Designation { Value = "Patricio Sharply" }, new Email { Value = "psharply0@yolasite.com" }, new PhoneNumber { Value = "6947302134" });
        Representative r1 = new Representative(Guid.NewGuid(), 319982093, new Designation { Value = "Kayley Begbie" }, new Email { Value = "kbegbie1@spotify.com" }, new PhoneNumber { Value = "6382283741" });
        Representative r2 = new Representative(Guid.NewGuid(), 995128061, new Designation { Value = "Vivian Llewellin" }, new Email { Value = "vllewellin2@china.com" }, new PhoneNumber { Value = "2019448698" });
        Representative r3 = new Representative(Guid.NewGuid(), 889716996, new Designation { Value = "Salli Burren" }, new Email { Value = "sburren3@ustream.tv" }, new PhoneNumber { Value = "2153449398" });
        Representative r4 = new Representative(Guid.NewGuid(), 733060890, new Designation { Value = "Jasmina Willshear" }, new Email { Value = "jwillshear4@netscape.com" }, new PhoneNumber { Value = "8416077007" });
        Representative r5 = new Representative(Guid.NewGuid(), 608839632, new Designation { Value = "Dore Whytock" }, new Email { Value = "dwhytock5@epa.gov" }, new PhoneNumber { Value = "5226915311" });
        Representative r6 = new Representative(Guid.NewGuid(), 398096220, new Designation { Value = "Rand Broadbere" }, new Email { Value = "rbroadbere6@springer.com" }, new PhoneNumber { Value = "1614875657" });
        Representative r7 = new Representative(Guid.NewGuid(), 446072968, new Designation { Value = "Bernardo Ansty" }, new Email { Value = "bansty7@geocities.com" }, new PhoneNumber { Value = "3507887407" });

        // Add Bootstrap data
        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Global Shipping Co." },
                new List<Designation> { new Designation { Value = "GSC" }, new Designation { Value = "Global Ship" } },
                new Address("123 Ocean Drive", "Maritime City", "USA", "90210"),
                new TaxNumber { Value = "TAX123456" },
                new List<Representative> { r, r1 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Oceanic Freight Ltd." },
                new List<Designation> { new Designation { Value = "OFL" }, new Designation { Value = "Oceanic Freight" } },
                new Address("456 Harbor Road", "Seaside Town", "UK", "AB12 3CD"),
                new TaxNumber { Value = "TAX654321" },
                new List<Representative> { r2, r3 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "TransWorld Logistics" },
                new List<Designation> { new Designation { Value = "TWL" }, new Designation { Value = "TransWorld" } },
                new Address("789 Dockside Ave", "Port City", "Canada", "A1B 2C3"),
                new TaxNumber { Value = "TAX789012" },
                new List<Representative> { r4, r5 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Maritime Movers Inc." },
                new List<Designation> { new Designation { Value = "MMI" }, new Designation { Value = "Maritime Movers" } },
                new Address("321 Bay Street", "Coastal Village", "Australia", "2000"),
                new TaxNumber { Value = "TAX210987" },
                new List<Representative> { r6, r7 }
            )
        );

        context.SaveChanges();
    }

    private static void BootstrapVesselsAndDocks(ApiContext context)
    {

        VesselType vt1 = new VesselType(Guid.NewGuid(), new Designation { Value = "Panamax" }, new Designation { Value = "Max size for Panama Canal" }, 20, 10, 5);
        VesselType vt2 = new VesselType(Guid.NewGuid(), new Designation { Value = "Post-Panamax" }, new Designation { Value = "Larger than Panamax" }, 30, 15, 7);
        VesselType vt3 = new VesselType(Guid.NewGuid(), new Designation { Value = "Ultra Large Container Vessel (ULCV)" }, new Designation { Value = "Largest container ships" }, 40, 20, 10);
        VesselType vt4 = new VesselType(Guid.NewGuid(), new Designation { Value = "Handymax" }, new Designation { Value = "Medium-sized bulk carriers" }, 15, 8, 4);
        VesselType vt5 = new VesselType(Guid.NewGuid(), new Designation { Value = "Capesize" }, new Designation { Value = "Too large for Panama and Suez Canals" }, 50, 25, 12);

        // Check if there are any vessels already in the database
        if (context.Vessels.Any())
            return;

        // Add Bootstrap data
        context.Vessels.AddRange(
            new Vessel(Guid.NewGuid(), new Designation { Value = "Ever Given" }, new ImoNumber { Value = "IMO 7585229" },
            vt1, context.ShippingAgentOrganizations.First()),
            new Vessel(Guid.NewGuid(), new Designation { Value = "Maersk Triple E" }, new ImoNumber { Value = "IMO 3815389" },
            vt2, context.ShippingAgentOrganizations.Skip(1).First()),
            new Vessel(Guid.NewGuid(), new Designation { Value = "CMA CGM Marco Polo" }, new ImoNumber { Value = "IMO 6699530" },
            vt3, context.ShippingAgentOrganizations.Skip(2).First())
        );

        // Check if there are any docks already in the database
        if (context.Docks.Any())
            return;

        // Add Bootstrap data
        context.Docks.AddRange(
            new Dock(Guid.NewGuid(), new Designation { Value = "Dock A" }, new Designation { Value = "North Harbor" }, 500, 30, 15, new List<VesselType> { vt4, vt1 }),
            new Dock(Guid.NewGuid(), new Designation { Value = "Dock B" }, new Designation { Value = "East Harbor" }, 600, 35, 18, new List<VesselType> { vt5 }),
            new Dock(Guid.NewGuid(), new Designation { Value = "Dock C" }, new Designation { Value = "South Harbor" }, 700, 40, 20, new List<VesselType> { vt2, vt3 })
        );

        context.SaveChanges();
    }

    private static void BootstrapStorageAreas(ApiContext context)
    {
        Dock dock1 = context.Docks.First();
        Dock dock2 = context.Docks.Skip(1).First();
        Dock dock3 = context.Docks.Skip(2).First();

        HashSet<StorageArea.DockService> ds1 = new()
        {
            new StorageArea.DockService(dock1, 50),
            new StorageArea.DockService(dock2, 100)
        };

        HashSet<StorageArea.DockService> ds2 = new()
        {
            new StorageArea.DockService(dock2, 60),
            new StorageArea.DockService(dock3, 120)
        };

        StorageArea sa1 = new StorageArea(Guid.NewGuid(), new Code { Value = "YARD1" }, new Designation { Value = "North Yard" }, StorageAreaType.Yard, 1000, 200, ds1);
        StorageArea sa2 = new StorageArea(Guid.NewGuid(), new Code { Value = "YARD2" }, new Designation { Value = "South Yard" }, StorageAreaType.Yard, 1500, 300, ds2);
        StorageArea sa3 = new StorageArea(Guid.NewGuid(), new Code { Value = "WH1" }, new Designation { Value = "Main Warehouse" }, StorageAreaType.Warehouse, 2000, 500);
        StorageArea sa4 = new StorageArea(Guid.NewGuid(), new Code { Value = "WH2" }, new Designation { Value = "Secondary Warehouse" }, StorageAreaType.Warehouse, 1200, 400);

        // Check if there are any storage areas already in the database
        if (context.StorageAreas.Any())
            return;

        // Add Bootstrap data
        context.StorageAreas.AddRange(sa1, sa2, sa3, sa4);
        context.SaveChanges();
    }
}