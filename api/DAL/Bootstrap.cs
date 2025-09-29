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
        BootstrapSAOR(context);
        // Bootstrap Vessels and Vessel Types
        BootstrapVessels(context);

        context.SaveChanges();
    }

    private static void BootstrapQualifications(ApiContext context)
    {
        // Check if there are any qualifications already in the database
        if (context.Qualifications.Any())
            return;

        // Add Bootstrap data
        context.Qualifications.AddRange(
            new Qualification(Guid.NewGuid(), "STS Crane Operator"),
            new Qualification(Guid.NewGuid(), "Yard Crane Operator"),
            new Qualification(Guid.NewGuid(), "Truck Driver")
        );
    }

    private static void BootstrapSAOR(ApiContext context)
    {
        // Check if there are any representatives already in the database
        if (context.ShippingAgentOrganizations.Any())
            return;

        Representative r = new Representative(Guid.NewGuid(), 908029952, "Patricio Sharply", "psharply0@yolasite.com", "694-730-2134");
        Representative r1 = new Representative(Guid.NewGuid(), 319982093, "Kayley Begbie", "kbegbie1@spotify.com", "638-228-3741");
        Representative r2 = new Representative(Guid.NewGuid(), 995128061, "Vivian Llewellin", "vllewellin2@china.com", "201-944-8698");
        Representative r3 = new Representative(Guid.NewGuid(), 889716996, "Salli Burren", "sburren3@ustream.tv", "215-344-9398");
        Representative r4 = new Representative(Guid.NewGuid(), 733060890, "Jasmina Willshear", "jwillshear4@netscape.com", "841-607-7007");
        Representative r5 = new Representative(Guid.NewGuid(), 608839632, "Dore Whytock", "dwhytock5@epa.gov", "522-691-5311");
        Representative r6 = new Representative(Guid.NewGuid(), 398096220, "Rand Broadbere", "rbroadbere6@springer.com", "161-487-5657");
        Representative r7 = new Representative(Guid.NewGuid(), 446072968, "Bernardo Ansty", "bansty7@geocities.com", "350-788-7407");

        // Add Bootstrap data
        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                "Global Shipping Co.",
                new List<string> { "GSC", "Global Ship" },
                new Address("123 Ocean Drive", "Maritime City", "USA", "90210"),
                "TAX123456",
                new List<Representative> { r, r1 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                "Oceanic Freight Ltd.",
                new List<string> { "OFL", "Oceanic Freight" },
                new Address("456 Harbor Road", "Seaside Town", "UK", "AB12 3CD"),
                "TAX654321",
                new List<Representative> { r2, r3 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                "TransWorld Logistics",
                new List<string> { "TWLogistics", "TWorld" },
                new Address("789 Dockside Ave", "Port City", "Canada", "A1B 2C3"),
                "TAX789012",
                new List<Representative> { r4, r5 }
            )
        );

        context.ShippingAgentOrganizations.Add(
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                "Maritime Movers Inc.",
                new List<string> { "MMI", "Maritime Movers" },
                new Address("321 Bay Street", "Coastal Village", "Australia", "2000"),
                "TAX210987",
                new List<Representative> { r6, r7 }
            )
        );

    }

    private static void BootstrapVessels(ApiContext context)
    {
        // Check if there are any vessels already in the database
        if (context.Vessels.Any())
            return;

        // Add Bootstrap data
        context.Vessels.AddRange(
            new Vessel(Guid.NewGuid(), "Ever Given", "IMO1234567",
            new VesselType(Guid.NewGuid(), "Panamax", "Max size for Panama Canal", 20, 10, 5),
            252482890),
            new Vessel(Guid.NewGuid(), "Maersk Triple E", "IMO7654321",
            new VesselType(Guid.NewGuid(), "Post-Panamax", "Larger than Panamax", 30, 15, 7),
            252482890),
            new Vessel(Guid.NewGuid(), "CMA CGM Marco Polo", "IMO1122334",
            new VesselType(Guid.NewGuid(), "Ultra Large Container Vessel (ULCV)", "Largest container ships", 40, 20, 10),
            252482918)
        );
    }
}