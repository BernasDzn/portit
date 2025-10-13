using System.Text.Json;
using Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Domain.Entities;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using NuGet.Protocol;

namespace Tests.Integration;

public class VesselControllerTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public VesselControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the real database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApiContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database
                services.AddDbContext<ApiContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("/Vessel")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_ReturnData()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/Vessel");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var stringResponse = await response.Content.ReadAsStringAsync();
        var vessels = JsonSerializer.Deserialize<List<VesselDto>>(stringResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(vessels);
        Assert.IsType<List<VesselDto>>(vessels);
    }

    [Fact]
    public async Task Post_Vessel_Successfully()
    {
        var vt = new
        {
             Name = "Panamax",
                Description = "Max size for Panama Canal",
                MaxNumberOfRows = 20,
                MaxNumberOfBays = 10,
                MaxNumberOfTiers = 5,
                PhysicalCharacteristics = new PhysicalCharacteristicsDto
                {
                    Length = 300,
                    Depth = 35,
                    Draft = 20
                }
        };

        var response = await _client.PostAsJsonAsync("/VesselType", vt);

        response.EnsureSuccessStatusCode();

        var rep = new
        {
            Name = "Patricio Sharply",
            CitizenshipId = 908029952,
            Designation = "Patricio Sharply",
            EmailAddress = "psharply0@yolasite.com",
            Phone = "6947302134"
        };

        response = await _client.PostAsJsonAsync("/Representative", rep);
        response.EnsureSuccessStatusCode();

        var sao = new
        {
            Name = "Global Shipping Co.",
            AltNames = new string[] { "GSC", "GlobalShip" },
            TaxNumber = "123456789",
            Address = new Address("123 Test St", "Test City", "Test Country", "12345"),
            Representatives = new HashSet<object> { rep }
        };

        response = await _client.PostAsJsonAsync("/ShippingAgentOrganization", sao);
        response.EnsureSuccessStatusCode();

        // Arrange
        // Use an existing VesselType from bootstrap data (Panamax)
        var vesselDto = new
        {
            Name = "Test Vessel",
            ImoNumber = "IMO 4569858",
            PhysicalCharacteristics = new PhysicalCharacteristicsDto
            {
                Length = 250,
                Depth = 12,
                Draft = 10
            },
            Type = vt,
            Owner = sao
        };

        // Act - Create
        var postResponse = await _client.PostAsJsonAsync("/Vessel", vesselDto);

        // Assert - Create
        Console.WriteLine(await postResponse.Content.ReadAsStringAsync());
        postResponse.EnsureSuccessStatusCode(); // Status Code 200-299
        var createdVessel = await postResponse.Content.ReadFromJsonAsync<VesselDto>();
        Assert.NotNull(createdVessel);
        Assert.Equal(vesselDto.Name, createdVessel.Name);
        Assert.Equal(vesselDto.ImoNumber, createdVessel.ImoNumber);

        // Verify the vessel was added to the collection
        var getResponse = await _client.GetAsync("/Vessel");
        getResponse.EnsureSuccessStatusCode();
        var stringResponse = await getResponse.Content.ReadAsStringAsync();
        var vessels = JsonSerializer.Deserialize<List<VesselDto>>(stringResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(vessels);
        Assert.Contains(vessels, v => v.ImoNumber == vesselDto.ImoNumber);
    }

}
