using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Api.Infrastructure.Persistence;

namespace Tests.Integration;

public class IntegrationTestsApplicationFactory<Program>
    : WebApplicationFactory<Program> where Program : class
{

    public IntegrationTestsApplicationFactory()
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        // based on https://stackoverflow.com/questions/72679169/override-host-configuration-in-integration-testing-using-asp-net-core-6-minimal
        var configurationValues = new Dictionary<string, string?>
        {
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        builder
            // This configuration is used during the creation of the application
            // (e.g. BEFORE WebApplication.CreateBuilder(args) is called in Program.cs).
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                // This overrides configuration settings that were added as part 
                // of building the Host (e.g. calling WebApplication.CreateBuilder(args)).
                configurationBuilder.AddInMemoryCollection(configurationValues);
            });


        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApiContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            services.AddDbContext<ApiContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });
        });

        builder.UseEnvironment("Development");
    }
}