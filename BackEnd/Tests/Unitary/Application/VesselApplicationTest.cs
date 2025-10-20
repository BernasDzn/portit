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

public class VesselApplicationTest : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = $"TestDatabase_{Guid.NewGuid()}";
    private readonly HttpClient _client;

    public VesselApplicationTest()
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
                if (!context.Vessels.Any())
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
    public async Task CreateVessel_ReturnsCreatedResponse_WhenVesselIsValid()
    {
        // Arrange
        var body = @"{
            ""name"": ""Maersk Triple E MKII"",
            ""imoNumber"": ""IMO 8476310"",
            ""type"": {
                ""name"": ""Post-Panamax"",
                ""description"": ""Larger than Panamax"",
                ""maxNumberOfRows"": 30,
                ""maxNumberOfBays"": 15,
                ""maxNumberOfTiers"": 7,
                ""length"": 400,
                ""depth"": 18,
                ""draft"": 14
            },
            ""owner"": {
                ""name"": ""Global Shipping Co."",
                ""altNames"": [
                    ""GSC"",
                    ""Global Ship""
                ],
                ""taxNumber"": ""PT123456789"",
                ""address"": {
                    ""id"": ""69aa6e59-3213-4338-9f1b-e25fb196c794"",
                    ""street"": ""123 Ocean Drive"",
                    ""city"": ""Maritime City"",
                    ""zipCode"": ""90210"",
                    ""country"": ""USA""
                },
                ""representatives"": [
                    {
                        ""name"": ""Patricio Sharply"",
                        ""citizenshipId"": 908029952,
                        ""emailAddress"": ""psharply0@yolasite.com"",
                        ""phone"": ""6947302134""
                    },
                    {
                        ""name"": ""Kayley Begbie"",
                        ""citizenshipId"": 319982093,
                        ""emailAddress"": ""kbegbie1@spotify.com"",
                        ""phone"": ""6382283741""
                    }
                ]
            },
            ""length"": 370,
            ""depth"": 16,
            ""draft"": 13
        }";

        var request = new HttpRequestMessage(HttpMethod.Post, "/Vessel")
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
        };

        // Act
        var response = await _client.SendAsync(request);

        // Debug: Print response details if not successful
        if (response.StatusCode != System.Net.HttpStatusCode.Created)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Error Content: {errorContent}");
            
            // Fail with the actual error message for easier debugging
            Assert.Fail($"Expected Created but got {response.StatusCode}. Error: {errorContent}");
        }

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var createdVessel = await response.Content.ReadFromJsonAsync<VesselDto>();
        Assert.NotNull(createdVessel);
    }

    [Fact]
    public async Task GetVessels_ReturnsOkResponse_WithListOfVessels()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/Vessel");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var vessels = await response.Content.ReadFromJsonAsync<IEnumerable<VesselDto>>();
        Assert.NotNull(vessels);
        Assert.NotEmpty(vessels);
    }

    [Fact]
    public async Task GetVesselByImo_ReturnsOkResponse_WhenVesselExists()
    {
        // Arrange
        var imoNumber = "IMO 3815389"; // Existing IMO number from seeded data
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Vessel/{imoNumber}");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var vessel = await response.Content.ReadFromJsonAsync<VesselDto>();
        Assert.NotNull(vessel);
        Assert.Equal(imoNumber, vessel.ImoNumber);
    }

    [Fact]
    public async Task GetVesselByImo_ReturnsNotFoundResponse_WhenVesselDoesNotExist()
    {
        // Arrange
        var imoNumber = "IMO 0000000"; // Non-existing IMO number
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Vessel/{imoNumber}");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FilterVessels_ReturnsOkResponse_WithFilteredVessels()
    {
        // Arrange
        var filterQuery = "?typeName=Panamax&minLength=200&maxLength=300";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Vessel/filter{filterQuery}");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var pagedVessels = await response.Content.ReadFromJsonAsync<Page<VesselDto>>();
        Assert.NotNull(pagedVessels);
        Assert.NotEmpty(pagedVessels.Items);
    }
}