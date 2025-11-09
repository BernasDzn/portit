using Api.Domain.Entities;
using Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application;

public class BaseApplicationTest : WebApplicationFactory<Program>
{
    protected readonly string DatabaseName;
    protected readonly HttpClient Client;

    public BaseApplicationTest()
    {
        DatabaseName = $"TestDatabase_{Guid.NewGuid()}";
        Client = CreateClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add a shared in-memory database for all tests in this class
            services.AddDbContext<ApiContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });

            // Replace authentication with test authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        });

        builder.UseEnvironment("Testing");

        // Seed the database after configuration
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SystemUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<SystemUserRole>>();

            try
            {
                context.Database.EnsureCreated();

                // Only seed if database is empty (to avoid duplicate seeding)
                if (!context.Docks.Any())
                {
                    Api.Application.Bootstrap.InitAsync(context, userManager, roleManager, false).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
            }
        });
    }

    public new void Dispose()
    {
        Client?.Dispose();
        base.Dispose();
    }
}
