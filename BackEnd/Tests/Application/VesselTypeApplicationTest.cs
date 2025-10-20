using Api.Application.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Api.Infrastructure.Utilities;


namespace Tests.Unitary.Application;

public class VesselTypeApplicationTest : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = $"TestDatabase_{Guid.NewGuid()}";
    private readonly HttpClient _client;

    public VesselTypeApplicationTest()
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
    public async Task CreateVesselType_ReturnsCreatedResponse_WhenVesselTypeIsValid()
    {
        var body = @"{
            ""name"": ""Large Vessel"",
            ""description"": ""A vessel type for large ships."",
            ""maxNumberOfRows"": 30,
            ""maxNumberOfBays"": 15,
            ""maxNumberOfTiers"": 10,
            ""capacity"":0,
            ""physicalCharacteristics"": {
                ""length"": 300,
                ""depth"": 25,
                ""draft"": 10
            }
        }";

        var request = new HttpRequestMessage(HttpMethod.Post, "/VesselType")
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
        var createdVesselType = await response.Content.ReadFromJsonAsync<VesselTypeDto>();
        Assert.NotNull(createdVesselType);
    }

    [Fact]
    public async Task GetVesselTypes_ReturnsOkResponse_WithListOfVesselTypes()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/VesselType");

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var vesselTypes = await response.Content.ReadFromJsonAsync<IEnumerable<VesselTypeDto>>();
        Assert.NotNull(vesselTypes);
        Assert.NotEmpty(vesselTypes);
    }

    [Fact]
    public async Task GetVesselTypeByName_ReturnsOkResponse_WhenVesselTypeExists()
    {
        var name = "Panamax";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/VesselType/{name}");

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var vesselType = await response.Content.ReadFromJsonAsync<VesselTypeDto>();
        Assert.NotNull(vesselType);
        Assert.Equal(name, vesselType.Name);
    }

    [Fact]
    public async Task GetVesselTypeByName_ReturnsNotFoundResponse_WhenVesselTypeDoesNotExist()
    {
       
        var name = "Not Exist";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/VesselType/{name}");

        
        var response = await _client.SendAsync(request);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FilterVesselTypes_ReturnsOkResponse_WithFilteredVesselTypes()
    {
        
        var filterQuery = "?Name=Panamax&Description=Max%20size%20for%20Panama%20Canal";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/VesselType/filter{filterQuery}");

        
        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var pagedVesselTypes = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedVesselTypes);
        Assert.NotEmpty(pagedVesselTypes.Items);
    }
}