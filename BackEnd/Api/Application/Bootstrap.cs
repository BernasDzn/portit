namespace Api.Application;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Persistence;

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
        // Bootstrap Physical Resources
        BootstrapPhysicalResources(context);
        // Bootstrap Staff
        BootstrapStaff(context);
        // Bootstrap Vessel Visit Notifications
        BootstrapVVN(context);
        // Bootstrap Vessel Visit Notification Decisions
        BootstrapVVNDecision(context);
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
            new Qualification(Guid.NewGuid(), new Code { Value = "TRKDR" }, new Designation { Value = "Truck Driver" }),
            new Qualification(Guid.NewGuid(), new Code { Value = "YAPLN" }, new Designation { Value = "Yard Planner" })
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
                new HashSet<Representative> { r, r1 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Oceanic Freight Ltd." },
                new List<Designation> { new Designation { Value = "OFL" }, new Designation { Value = "Oceanic Freight" } },
                new Address("456 Harbor Road", "Seaside Town", "UK", "AB12 3CD"),
                new TaxNumber { Value = "TAX654321" },
                new HashSet<Representative> { r2, r3 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "TransWorld Logistics" },
                new List<Designation> { new Designation { Value = "TWL" }, new Designation { Value = "TransWorld" } },
                new Address("789 Dockside Ave", "Port City", "Canada", "A1B 2C3"),
                new TaxNumber { Value = "TAX789012" },
                new HashSet<Representative> { r4, r5 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Maritime Movers Inc." },
                new List<Designation> { new Designation { Value = "MMI" }, new Designation { Value = "Maritime Movers" } },
                new Address("321 Bay Street", "Coastal Village", "Australia", "2000"),
                new TaxNumber { Value = "TAX210987" },
                new HashSet<Representative> { r6, r7 }
            )
        );

        context.SaveChanges();
    }

    private static void BootstrapVesselsAndDocks(ApiContext context)
    {
        // Check if there are any docks already in the database
        if (context.Docks.Any())
            return;

        VesselType vt1 = new VesselType(Guid.NewGuid(), new Designation { Value = "Panamax" }, new Designation { Value = "Max size for Panama Canal" }, 20, 10, 5, new PhysicalCharacteristics { Length = 300, Depth = 15, Draft = 12 });
        VesselType vt2 = new VesselType(Guid.NewGuid(), new Designation { Value = "Post-Panamax" }, new Designation { Value = "Larger than Panamax" }, 30, 15, 7, new PhysicalCharacteristics { Length = 400, Depth = 18, Draft = 14 });
        VesselType vt3 = new VesselType(Guid.NewGuid(), new Designation { Value = "Ultra Large Container Vessel (ULCV)" }, new Designation { Value = "Largest container ships" }, 40, 20, 10, new PhysicalCharacteristics { Length = 500, Depth = 25, Draft = 18 });
        VesselType vt4 = new VesselType(Guid.NewGuid(), new Designation { Value = "Handymax" }, new Designation { Value = "Medium-sized bulk carriers" }, 15, 8, 4, new PhysicalCharacteristics { Length = 250, Depth = 12, Draft = 10 });
        VesselType vt5 = new VesselType(Guid.NewGuid(), new Designation { Value = "Capesize" }, new Designation { Value = "Too large for Panama and Suez Canals" }, 50, 25, 12, new PhysicalCharacteristics { Length = 600, Depth = 30, Draft = 20 });

        // Check if there are any vessels already in the database
        if (context.Vessels.Any())
            return;

        // Add Bootstrap data
        context.Vessels.AddRange(
            new Vessel(Guid.NewGuid(), new Designation { Value = "Ever Given" }, new ImoNumber { Value = "IMO 7585229" },
            vt1, context.ShippingAgentOrganizations.First(), new PhysicalCharacteristics { Length = 270, Depth = 13, Draft = 10 }),
            new Vessel(Guid.NewGuid(), new Designation { Value = "Maersk Triple E" }, new ImoNumber { Value = "IMO 3815389" },
            vt2, context.ShippingAgentOrganizations.Skip(1).First(), new PhysicalCharacteristics { Length = 370, Depth = 16, Draft = 13 }),
            new Vessel(Guid.NewGuid(), new Designation { Value = "CMA CGM Marco Polo" }, new ImoNumber { Value = "IMO 6699530" },
            vt3, context.ShippingAgentOrganizations.Skip(2).First(), new PhysicalCharacteristics { Length = 470, Depth = 24, Draft = 15 })
        );

        // Add Bootstrap data
        context.Docks.AddRange(
            new Dock(Guid.NewGuid(), new Designation { Value = "Dock A" }, new Designation { Value = "North Harbor" }, new PhysicalCharacteristics { Length = 500, Depth = 35, Draft = 20 }, new HashSet<VesselType> { vt4, vt1 }),
            new Dock(Guid.NewGuid(), new Designation { Value = "Dock B" }, new Designation { Value = "East Harbor" }, new PhysicalCharacteristics { Length = 700, Depth = 35, Draft = 20 }, new HashSet<VesselType> { vt5 }),
            new Dock(Guid.NewGuid(), new Designation { Value = "Dock C" }, new Designation { Value = "South Harbor" }, new PhysicalCharacteristics { Length = 700, Depth = 40, Draft = 25 }, new HashSet<VesselType> { vt2, vt3 })
        );

        context.SaveChanges();
    }

    private static void BootstrapStorageAreas(ApiContext context)
    {
        // Check if there are any storage areas already in the database
        if (context.StorageAreas.Any())
            return;

        Dock dock1 = context.Docks.First();
        Dock dock2 = context.Docks.Skip(1).First();
        Dock dock3 = context.Docks.Skip(2).First();

        HashSet<StorageArea.DockRelation> ds1 = new()
        {
            new StorageArea.DockRelation(dock1, null, false),
            new StorageArea.DockRelation(dock2, 100, false)
        };

        HashSet<StorageArea.DockRelation> ds2 = new()
        {
            new StorageArea.DockRelation(dock2, 60, true),
            new StorageArea.DockRelation(dock3, 120, false)
        };

        StorageArea sa1 = new StorageArea(Guid.NewGuid(), new Code { Value = "YARD1" }, new Designation { Value = "North Yard" }, StorageAreaType.Yard, 1000, 200, ds1);
        StorageArea sa2 = new StorageArea(Guid.NewGuid(), new Code { Value = "YARD2" }, new Designation { Value = "South Yard" }, StorageAreaType.Yard, 1500, 300, ds2);
        StorageArea sa3 = new StorageArea(Guid.NewGuid(), new Code { Value = "WH1" }, new Designation { Value = "Main Warehouse" }, StorageAreaType.Warehouse, 2000, 500, []);
        StorageArea sa4 = new StorageArea(Guid.NewGuid(), new Code { Value = "WH2" }, new Designation { Value = "Secondary Warehouse" }, StorageAreaType.Warehouse, 1200, 400, []);

        // Add Bootstrap data
        context.StorageAreas.AddRange(sa1, sa2, sa3, sa4);
        context.SaveChanges();
    }

    private static void BootstrapPhysicalResources(ApiContext context)
    {
        // Check if there are any physical resources already in the database
        if (context.PhysicalResources.Any())
            return;

        Qualification stsOp = context.Qualifications.First(q => q.NameCode.Value == "STSOP");
        Qualification ycOp = context.Qualifications.First(q => q.NameCode.Value == "YACOP");
        Qualification trkDr = context.Qualifications.First(q => q.NameCode.Value == "TRKDR");

        STSCrane crane1 = new STSCrane(
            Guid.NewGuid(),
            new Code { Value = "STS001" },
            new Designation { Value = "STS Crane 1" },
            ResourceStatus.Available,
            TimeSpan.FromMinutes(30),
            new HashSet<Qualification> { stsOp },
            OperationalWindow.FullWeek(),
            40,
            context.Docks.First(),
            30
        );

        STSCrane crane2 = new STSCrane(
            Guid.NewGuid(),
            new Code { Value = "STS002" },
            new Designation { Value = "STS Crane 2" },
            ResourceStatus.Maintenance,
            TimeSpan.FromMinutes(45),
            new HashSet<Qualification> { stsOp },
            OperationalWindow.FullWeek(),
            50,
            context.Docks.Skip(1).First(),
            25
        );

        YardCrane yardCrane1 = new YardCrane(
            Guid.NewGuid(),
            new Code { Value = "YC001" },
            new Designation { Value = "Yard Crane 1" },
            ResourceStatus.Available,
            TimeSpan.FromMinutes(20),
            new HashSet<Qualification> { ycOp },
            OperationalWindow.Weekdays(new TimeOnly(8, 0), new TimeOnly(18, 0)),
            20,
            context.StorageAreas.First(sa => sa.AreaType == StorageAreaType.Yard),
            40
        );

        YardCrane yardCrane2 = new YardCrane(
            Guid.NewGuid(),
            new Code { Value = "YC002" },
            new Designation { Value = "Yard Crane 2" },
            ResourceStatus.OutOfService,
            TimeSpan.FromMinutes(25),
            new HashSet<Qualification> { ycOp },
            OperationalWindow.FullWeek(),
            25,
            context.StorageAreas.First(sa => sa.AreaType == StorageAreaType.Yard),
            35
        );

        Truck truck1 = new Truck(
            Guid.NewGuid(),
            new Code { Value = "TRK001" },
            new Designation { Value = "Truck 1" },
            ResourceStatus.Available,
            TimeSpan.FromMinutes(15),
            new HashSet<Qualification> { trkDr },
            OperationalWindow.Weekdays(new TimeOnly(6, 0), new TimeOnly(22, 0)),
            30,
            2,
            80
        );

        Truck truck2 = new Truck(
            Guid.NewGuid(),
            new Code { Value = "TRK002" },
            new Designation { Value = "Truck 2" },
            ResourceStatus.Maintenance,
            TimeSpan.FromMinutes(20),
            new HashSet<Qualification> { trkDr },
            OperationalWindow.FullWeek(),
            25,
            1,
            50
        );

        context.PhysicalResources.AddRange(crane1, crane2, yardCrane1, yardCrane2, truck1, truck2);
        context.SaveChanges();
    }


    public static void BootstrapStaff(ApiContext context)
    {
        if (context.Staffs.Any())
            return;

        var qual1 = context.Qualifications.First();
        var qual2 = context.Qualifications.Skip(1).First();

        Staff staff1 = new Staff(
            new StaffMechanograficNumber { Value = "OCEANPMEC001" },
            new Designation { Value = "João Pedro" },
            new Email { Value = "joao.pedro@oceanicport.com" },
            new PhoneNumber { Value = "911222333" },
            OperationalWindow.Weekdays(new TimeOnly(8, 0), new TimeOnly(17, 0)),
            new List<Qualification> { qual1, qual2 }
        );

        Staff staff2 = new Staff(
            new StaffMechanograficNumber { Value = "OCEANPMEC002" },
            new Designation { Value = "Maria Silva" },
            new Email { Value = "maria.silva@oceanicport.com" },
            new PhoneNumber { Value = "911222444" },
            OperationalWindow.Weekdays(new TimeOnly(8, 0), new TimeOnly(17, 0)),
            new List<Qualification> { qual1, qual2 }
        );

        Staff staff3 = new Staff(
            new StaffMechanograficNumber { Value = "OCEANPMEC003" },
            new Designation { Value = "Carlos Santos" },
            new Email { Value = "carlos.santos@oceanicport.com" },
            new PhoneNumber { Value = "911222555" },
            OperationalWindow.Weekdays(new TimeOnly(10, 0), new TimeOnly(18, 0)),
            new List<Qualification> { qual1 }
        );

        context.Staffs.AddRange(staff1, staff2, staff3);
        context.SaveChanges();
    }

    public static void BootstrapVVN(ApiContext context)
    {

        if (context.VesselVisitNotifications.Any())
            return;

        var vessel1 = context.Vessels.First();
        var vessel2 = context.Vessels.Skip(1).First();
        var vessel3 = context.Vessels.Skip(2).First();

        VesselVisitNotification vvn1 = new VesselVisitNotification(
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5),
            false,
            vessel1,
            context.ShippingAgentOrganizations.First().Representatives.First(),
            "Requires additional security measures",
            new Crew("Mario Silva", 5, new HashSet<SafetyOfficer>())
        );

        VesselVisitNotification vvn2 = new VesselVisitNotification(
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(10),
            false,
            vessel2,
            context.ShippingAgentOrganizations.First().Representatives.First()
        );

        HashSet<SafetyOfficer> safetyOfficers = new HashSet<SafetyOfficer>
        {
            new SafetyOfficer{CitizenID = "CITIZEN001", Name = "John Doe", Nationality = "American" },
            new SafetyOfficer{CitizenID = "CITIZEN002", Name = "Jane Smith", Nationality = "British" }
        };

        Crew crewDetails = new Crew("Ana Costa", 3, safetyOfficers);

        VesselVisitNotification vvn3 = new VesselVisitNotification(
            DateTime.UtcNow.AddDays(12),
            DateTime.UtcNow.AddDays(15),
            true,
            vessel3,
            context.ShippingAgentOrganizations.First().Representatives.First(),
            "Handles hazardous materials",
            crewDetails
        );

        context.VesselVisitNotifications.AddRange(vvn1, vvn2, vvn3);
        context.SaveChanges();
    }

    public static void BootstrapVVNDecision(ApiContext context)
    {
        if (context.NotificationDecisions.Any() || !context.VesselVisitNotifications.Any())
            return;

        var vvn1 = context.VesselVisitNotifications.First();
        var vvn2 = context.VesselVisitNotifications.Skip(1).First();
        var vvn3 = context.VesselVisitNotifications.Skip(2).First();

        NotificationDecision decision1 = new NotificationDecision(
            NotificationDecisionStatus.In_Progress,
            DateTime.UtcNow,
            vvn1);

        NotificationDecision decision2 = new NotificationDecision(
            NotificationDecisionStatus.Rejected,
            DateTime.UtcNow,
            vvn2,
            null,
            null,
            "Vessel does not meet safety requirements."
        );

        NotificationDecision decision3 = new NotificationDecision(
            NotificationDecisionStatus.Approved,
            DateTime.UtcNow,
            vvn3,
            null,
            context.Docks.First()
        );

        context.NotificationDecisions.AddRange(decision1, decision2, decision3);
        context.SaveChanges();
    }

}