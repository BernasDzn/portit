using Api.Application.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Api.Infrastructure.Utilities;


namespace Tests.Application;

public class DockApplicationTest : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = $"TestDatabase_{Guid.NewGuid()}";
    private readonly HttpClient _client;

    public DockApplicationTest()
    {
        _client = CreateClient();
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
        });

        builder.UseEnvironment("Testing");
        
        // Seed the database after configuration
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiContext>();
            
            try
            {
                context.Database.EnsureCreated();
                
                // Only seed if database is empty (to avoid duplicate seeding)
                if (!context.Docks.Any())
                {
                    Api.Application.Bootstrap.Init(context, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
            }
        });
    }

    [Fact]
    public async Task CreateDock_ReturnsCreatedResponse_WhenDockIsValid()
    {
        
        var body = @"{
            ""code"" : ""DCK004"",
            ""name"": ""Dock D"",
            ""location"": ""North Harbor"",
            ""physicalCharacteristics"": {
                ""length"": 500,
                ""depth"": 35,
                ""draft"": 20
            },
            ""supportedVesselTypes"": [""Panamax"", ""Handymax""]
        }";

        var request = new HttpRequestMessage(HttpMethod.Post, "/Dock")
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
        };

        
        var response = await _client.SendAsync(request);


        if (response.StatusCode != System.Net.HttpStatusCode.Created)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Error Content: {errorContent}");


            Assert.Fail($"Expected Created but got {response.StatusCode}. Error: {errorContent}");
        }

        
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var createdDock = await response.Content.ReadFromJsonAsync<DockDto>();
        Assert.NotNull(createdDock);
    }

    [Fact]
    public async Task GetDocks_ReturnsOkResponse_WithListOfDocks()
    {
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/Dock");

        
        var response = await _client.SendAsync(request);

        
        response.EnsureSuccessStatusCode();
        var docks = await response.Content.ReadFromJsonAsync<IEnumerable<DockDto>>();
        Assert.NotNull(docks);
        Assert.NotEmpty(docks);
    }

    [Fact]
    public async Task GetDockByCode_ReturnsOkResponse_WhenDockExists()
    {
        
        var code = "DCK004";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Dock/{code}");

    
        var response = await _client.SendAsync(request);

        
        response.EnsureSuccessStatusCode();
        var dock = await response.Content.ReadFromJsonAsync<DockDto>();
        Assert.NotNull(dock);
        Assert.Equal(code, dock.Code);
    }

    [Fact]
    public async Task GetDockByCode_ReturnsNotFoundResponse_WhenDockDoesNotExist()
    {
        
        var code = "DCK0000"; // Non-existing Dock code
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Dock/{code}");

        
        var response = await _client.SendAsync(request);

        
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FilterDocks_ReturnsOkResponse_WithFilteredDocks()
    {
        
        var filterQuery = "?DockName=Dock%20A&Location=North%20Harbor&VesselTypeName=Handymax";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Dock/filter{filterQuery}");

        
        var response = await _client.SendAsync(request);

        
        response.EnsureSuccessStatusCode();
        var pagedDocks = await response.Content.ReadFromJsonAsync<Page<DockDto>>();
        Assert.NotNull(pagedDocks);
        Assert.NotEmpty(pagedDocks.Items);
    }
}