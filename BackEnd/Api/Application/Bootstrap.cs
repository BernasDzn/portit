namespace Api.Application;

using System.Globalization;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Persistence;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;

public static class Bootstrap
{
    public static async Task InitAsync(ApiContext context, UserManager<SystemUser> userManager, RoleManager<SystemUserRole> roleManager, bool nukeDatabase = false)
    {
        // Delete the database if we need that
        if (nukeDatabase)
            context.Database.EnsureDeleted();

        // Ensure the database is created
        context.Database.EnsureCreated();

        await BootstrapRolesAsync(roleManager);
        await BootstrapUsersAsync(context, userManager);

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
    }

    private static async Task BootstrapRolesAsync(RoleManager<SystemUserRole> roleManager)
    {
        // Create all roles if they don't exist
        var roleTypes = Enum.GetValues<SystemUserRoleType>();
        foreach (var roleType in roleTypes)
        {
            var roleName = roleType.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new SystemUserRole(roleName));
            }
        }
    }

    private static async Task BootstrapUsersAsync(ApiContext context, UserManager<SystemUser> userManager)
    {
        // Check if there are any users already in the database
        if (context.Users.Any())
            return;

        var users = new[]
        {
            new SystemUser("bernardogranjacardoso@gmail.com")
            {
                Sub = "109730045337224782667",
                Active = true
            },
            new SystemUser("franciscolousada19@gmail.com")
            {
                Sub = "111839515929489386087",
                Active = true
            },
            new SystemUser("ruisantiago.jp@gmail.com")
            {
                Sub = "114350264242626181256",
                Active = true
            },
            new SystemUser("tiagobarrossao@gmail.com")
            {
                Sub = "111642040238696442904",
                Active = true
            }
        };

        foreach (var user in users)
        {
            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, SystemUserRoleType.Administrator.ToString());
        }
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
        Representative r2 = new Representative(Guid.NewGuid(), 995128061, new Designation { Value = "Vivian Llewellin" }, new Email { Value = "u0242497470@gmail.com" }, new PhoneNumber { Value = "2019448698" });
        Representative r3 = new Representative(Guid.NewGuid(), 889716996, new Designation { Value = "Salli Burren" }, new Email { Value = "sburren3@ustream.tv" }, new PhoneNumber { Value = "2153449398" });
        Representative r4 = new Representative(Guid.NewGuid(), 733060890, new Designation { Value = "Jasmina Willshear" }, new Email { Value = "jwillshear4@netscape.com" }, new PhoneNumber { Value = "8416077007" });
        Representative r5 = new Representative(Guid.NewGuid(), 608839632, new Designation { Value = "Dore Whytock" }, new Email { Value = "dwhytock5@epa.gov" }, new PhoneNumber { Value = "5226915311" });
        Representative r6 = new Representative(Guid.NewGuid(), 398096220, new Designation { Value = "Rand Broadbere" }, new Email { Value = "rbroadbere6@springer.com" }, new PhoneNumber { Value = "1614875657" });
        Representative r7 = new Representative(Guid.NewGuid(), 446072968, new Designation { Value = "Bernardo Ansty" }, new Email { Value = "bernasdzn@gmail.com" }, new PhoneNumber { Value = "3507887407" });
        Representative r8 = new Representative(Guid.NewGuid(), 889716996, new Designation { Value = "Francis The III" }, new Email { Value = "numcheixd@gmail.com" }, new PhoneNumber { Value = "2153449398" });

        // Add Bootstrap data
        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Global Shipping Co." },
                new List<Designation> { new Designation { Value = "GSC" }, new Designation { Value = "Global Ship" } },
                new Address("123 Ocean Drive", "Maritime City", "90210", "USA"),
                new TaxNumber { Value = "PT123456789" },
                new HashSet<Representative> { r, r1, r8 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Oceanic Freight Ltd." },
                new List<Designation> { new Designation { Value = "OFL" }, new Designation { Value = "Oceanic Freight" } },
                new Address("456 Harbor Road", "Seaside Town", "AB12 3CD", "UK"),
                new TaxNumber { Value = "GB123456789" },
                new HashSet<Representative> { r2, r3 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "TransWorld Logistics" },
                new List<Designation> { new Designation { Value = "TWL" }, new Designation { Value = "TransWorld" } },
                new Address("789 Dockside Ave", "Port City", "A1B 2C3", "Canada"),
                new TaxNumber { Value = "PT987654321" },
                new HashSet<Representative> { r4, r5 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Maritime Movers Inc." },
                new List<Designation> { new Designation { Value = "MMI" }, new Designation { Value = "Maritime Movers" } },
                new Address("321 Bay Street", "Coastal Village", "2000", "Australia"),
                new TaxNumber { Value = "PT192837465" },
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
            vt3, context.ShippingAgentOrganizations.Skip(2).First(), new PhysicalCharacteristics { Length = 470, Depth = 24, Draft = 15 }),
            new Vessel(Guid.NewGuid(), new Designation { Value = "MSC Oscar" }, new ImoNumber { Value = "IMO 9703318" },
            vt4, context.ShippingAgentOrganizations.Skip(3).First(), new PhysicalCharacteristics { Length = 200, Depth = 10, Draft = 10 })
        );

        // Add Bootstrap data
        context.Docks.AddRange(
            new Dock(Guid.NewGuid(),new Code{Value = "DCK001"}, new Designation { Value = "Dock A" }, new Designation { Value = "North Harbor" }, new PhysicalCharacteristics { Length = 500, Depth = 35, Draft = 20 }, new HashSet<VesselType> { vt4, vt1 }),
            new Dock(Guid.NewGuid(),new Code{Value = "DCK002"}, new Designation { Value = "Dock B" }, new Designation { Value = "East Harbor" }, new PhysicalCharacteristics { Length = 700, Depth = 35, Draft = 20 }, new HashSet<VesselType> { vt5 }),
            new Dock(Guid.NewGuid(),new Code{Value = "DCK003"}, new Designation { Value = "Dock C" }, new Designation { Value = "South Harbor" }, new PhysicalCharacteristics { Length = 700, Depth = 40, Draft = 25 }, new HashSet<VesselType> { vt2, vt3 })
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

        StorageArea sa1 = new StorageArea(Guid.NewGuid(), new Code { Value = "YARD01" }, new Designation { Value = "North Yard" }, StorageAreaType.Yard, 1000, 200, ds1);
        StorageArea sa2 = new StorageArea(Guid.NewGuid(), new Code { Value = "YARD02" }, new Designation { Value = "South Yard" }, StorageAreaType.Yard, 1500, 300, ds2);
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
        var qual3 = context.Qualifications.Skip(2).First();
        var qual4 = context.Qualifications.Skip(3).First();

        Staff staff1 = new Staff(
            new StaffMechanographicNumber { Value = "STF250001" },
            new Designation { Value = "João Pedro" },
            new Email { Value = "joao.pedro@oceanicport.com" },
            new PhoneNumber { Value = "911222333" },
            OperationalWindow.Weekdays(new TimeOnly(8, 0), new TimeOnly(17, 0)),
            new List<Qualification> { qual1, qual2 }
        );

        Staff staff2 = new Staff(
            new StaffMechanographicNumber { Value = "STF250002" },
            new Designation { Value = "Maria Silva" },
            new Email { Value = "maria.silva@oceanicport.com" },
            new PhoneNumber { Value = "911222444" },
            OperationalWindow.Weekdays(new TimeOnly(8, 0), new TimeOnly(17, 0)),
            new List<Qualification> { qual2, qual3 }
        );

        Staff staff3 = new Staff(
            new StaffMechanographicNumber { Value = "STF250003" },
            new Designation { Value = "Carlos Santos" },
            new Email { Value = "carlos.santos@oceanicport.com" },
            new PhoneNumber { Value = "911222555" },
            OperationalWindow.Weekdays(new TimeOnly(10, 0), new TimeOnly(18, 0)),
            new List<Qualification> { qual1, qual4 }
        );

        context.Staffs.AddRange(staff1, staff2, staff3);
        context.SaveChanges();
    }

    public static void BootstrapVVN(ApiContext context)
    {
        // Conatiner ids validos gerados pelo engenheiro:
        /*
            - MSKU1234565
            - TGHU7654320
            - CMAU0000014
            - MAEU9999991
        */

        if (context.VesselVisitNotifications.Any())
            return;

        var vessel1 = context.Vessels.First();
        var vessel2 = context.Vessels.Skip(1).First();
        var vessel3 = context.Vessels.Skip(2).First();
        var vessel4 = context.Vessels.Skip(3).First();

        var sa1 = context.StorageAreas.First(sa => sa.AreaType == StorageAreaType.Yard);

        ICollection<CargoTransport> UnloadCargoManifest = new List<CargoTransport>
        {
            new CargoTransport(
                new ContainerPosition { Row = "10", Bay = "5", Tier = "10"},
                sa1,
                new Container(
                    Guid.NewGuid(),
                    new ContainerNumber { Value = "CMAU2468103" },
                    CargoType.GENERAL_CONSUMER_PRODUCTS,
                    new Designation { Value = "chilly yummy food" }
                )
            ),
            new CargoTransport(
                new ContainerPosition { Row = "12", Bay = "6", Tier = "8" },
                sa1,
                new Container(
                    Guid.NewGuid(),
                    new ContainerNumber { Value = "ABCD1234560" },
                    CargoType.ELECTRONICS,
                    new Designation { Value = "various electronic items" }
                )
            )
        };

        VesselVisitNotification vvn1 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                1,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5),
            false,
            vessel1,
            vessel1.Owner.Representatives.First(),
            "Requires additional security measures",
            new Crew(new Designation { Value = "Miguel Oliveira" }, 2, new HashSet<SafetyOfficer>()),
            null,
            UnloadCargoManifest
        );

        VesselVisitNotification vvn2 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                2,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(10),
            false,
            vessel2,
            vessel2.Owner.Representatives.First()
        );

        HashSet<SafetyOfficer> safetyOfficers = new HashSet<SafetyOfficer>
        {
            new SafetyOfficer{CitizenID = "CITIZEN001", Name = "John Doe", Nationality = "US" },
            new SafetyOfficer{CitizenID = "CITIZEN002", Name = "Jane Smith", Nationality = "GB" }
        };

        Crew crewDetails = new Crew(new Designation { Value = "Ana Costa" }, 3, safetyOfficers);

        VesselVisitNotification vvn3 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                3,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.UtcNow.AddDays(12),
            DateTime.UtcNow.AddDays(15),
            true,
            vessel3,
            vessel3.Owner.Representatives.First(),
            "Handles hazardous materials",
            crewDetails
        );

        Crew crewDetails2 = new Crew(new Designation { Value = "Pedro Gomes" }, 4, safetyOfficers);

        VesselVisitNotification vvn4 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                4,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.UtcNow.AddDays(12),
            DateTime.UtcNow.AddDays(15),
            true,
            vessel4,
            vessel4.Owner.Representatives.First(),
            "Handles hazardous materials",
            crewDetails2
        );

        VesselVisitNotification vvn5 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                5,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.UtcNow.AddDays(20),
            DateTime.UtcNow.AddDays(25),
            false,
            vessel1,
            vessel1.Owner.Representatives.First()
        );

        // 4 VVns for day 10/11/2024 for testing the scheduling
        VesselVisitNotification vvn6 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                6,
                2025
            ),
            new DateTime(2025, 11, 10, 8, 0, 0),
            new DateTime(2025, 11, 15, 18, 0, 0),
            false,
            vessel2,
            vessel2.Owner.Representatives.First(),
            null, null,
            new List<CargoTransport>
            {
                new CargoTransport(
                    new ContainerPosition { Row = "14", Bay = "7", Tier = "9" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "TGHU7654324" },
                        CargoType.OVERSIZED_INDUSTRIAL_EQUIPMENT,
                        new Designation { Value = "industrial machinery" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "16", Bay = "8", Tier = "6" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "MAEU9999993" },
                        CargoType.OTHER,
                        new Designation { Value = "chemical products" }
                    )
                )
            },
            new List<CargoTransport>()
            {
                new CargoTransport(
                    new ContainerPosition { Row = "18", Bay = "9", Tier = "5" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "MSKU1234562" },
                        CargoType.GENERAL_CONSUMER_PRODUCTS,
                        new Designation { Value = "assorted goods" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "20", Bay = "10", Tier = "4" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "CMAU0000017" },
                        CargoType.ELECTRONICS,
                        new Designation { Value = "various electronic items" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "22", Bay = "11", Tier = "3" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "ABCD1234562" },
                        CargoType.ELECTRONICS,
                        new Designation { Value = "various electronic items" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "24", Bay = "12", Tier = "2" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "CMAU2468102" },
                        CargoType.GENERAL_CONSUMER_PRODUCTS,
                        new Designation { Value = "chilly yummy food" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "26", Bay = "13", Tier = "1" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "EFGH6543215" },
                        CargoType.OTHER,
                        new Designation { Value = "miscellaneous items" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "28", Bay = "14", Tier = "0" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "IJKL0987658" },
                        CargoType.GENERAL_CONSUMER_PRODUCTS,
                        new Designation { Value = "various goods" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "30", Bay = "15", Tier = "5" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "MNOP5678908" },
                        CargoType.ELECTRONICS,
                        new Designation { Value = "electronic devices" }
                    )
                )
            }
        );

        VesselVisitNotification vvn7 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                7,
                2025
            ),
            new DateTime(2025, 11, 10, 9, 0, 0),
            new DateTime(2025, 11, 14, 17, 0, 0),
            false,
            vessel3,
            vessel3.Owner.Representatives.First(),
            null, null,
            new List<CargoTransport>()
            {
                new CargoTransport(
                    new ContainerPosition { Row = "28", Bay = "14", Tier = "6" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "EFGH6543215" },
                        CargoType.OTHER,
                        new Designation { Value = "chemical products" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "30", Bay = "15", Tier = "7" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "IJKL0987658" },
                        CargoType.OVERSIZED_INDUSTRIAL_EQUIPMENT,
                        new Designation { Value = "industrial machinery" }
                    )
                )
            },
            new List<CargoTransport>()
            {
                new CargoTransport(
                    new ContainerPosition { Row = "32", Bay = "16", Tier = "4" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "ZXCV1122339" },
                        CargoType.GENERAL_CONSUMER_PRODUCTS,
                        new Designation { Value = "assorted goods" }
                    )
                ),
                new CargoTransport(
                    new ContainerPosition { Row = "34", Bay = "17", Tier = "3" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "QWER5566774" },
                        CargoType.ELECTRONICS,
                        new Designation { Value = "various electronic items" }
                    )
                )
            }
        );

        VesselVisitNotification vvn8 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                8,
                2025
            ),
            new DateTime(2025, 11, 10, 10, 0, 0),
            new DateTime(2025, 11, 13, 16, 0, 0),
            false,
            vessel4,
            vessel4.Owner.Representatives.First(),
            null, null,
            new List<CargoTransport>()
            {
                new CargoTransport(
                    new ContainerPosition { Row = "36", Bay = "18", Tier = "8" },
                    sa1,
                    new Container(
                        Guid.NewGuid(),
                        new ContainerNumber { Value = "TYUI7788993" },
                        CargoType.OVERSIZED_INDUSTRIAL_EQUIPMENT,
                        new Designation { Value = "industrial machinery" }
                    )
                )
            }
        );

        VesselVisitNotification vvn9 = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                9,
                2025
            ),
            new DateTime(2025, 11, 10, 11, 0, 0),
            new DateTime(2025, 11, 12, 15, 0, 0),
            false,
            vessel1,
            vessel1.Owner.Representatives.First()
        );

        vvn6.Submit();
        vvn7.Submit();
        vvn8.Submit();
        vvn9.Submit();

        vvn6.AddDecision(NotificationDecisionFactory.CreateAccepted(context.Users.First().Email!, "All criteria met", context.Docks.First()));
        vvn7.AddDecision(NotificationDecisionFactory.CreateAccepted(context.Users.First().Email!, "All criteria met", context.Docks.First()));
        vvn8.AddDecision(NotificationDecisionFactory.CreateAccepted(context.Users.Skip(1).First().Email!, "Approved after review", context.Docks.Skip(2).First()));
        vvn9.AddDecision(NotificationDecisionFactory.CreateAccepted(context.Users.Skip(2).First().Email!, "All criteria met", context.Docks.First()));

        context.VesselVisitNotifications.AddRange(vvn6, vvn7, vvn8, vvn9);

        NotificationDecision decision1 = NotificationDecisionFactory.CreateAccepted(context.Users.First().Email!, "All criteria met", context.Docks.First());
        NotificationDecision decision2 = NotificationDecisionFactory.CreateRejected(context.Users.Skip(1).First().Email!, "Insufficient documentation", false);
        NotificationDecision decision3 = NotificationDecisionFactory.CreateAccepted(context.Users.Skip(3).First().Email!, "Approved after review", context.Docks.Skip(2).First());
        NotificationDecision decision4 = NotificationDecisionFactory.CreateRejected(context.Users.Skip(3).First().Email!, "Safety concerns", true);

        vvn1.Submit();
        vvn1.AddDecision(decision1);

        vvn2.Submit();
        vvn2.AddDecision(decision2);
        vvn2.Submit();
        vvn2.AddDecision(decision3);
        vvn3.Submit();
        vvn4.Submit();

        context.VesselVisitNotifications.AddRange(vvn1, vvn2, vvn3, vvn4, vvn5);
        context.SaveChanges();
    }

}