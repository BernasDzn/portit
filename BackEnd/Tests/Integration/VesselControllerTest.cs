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
        // Arrange
        // Use an existing VesselType from bootstrap data (Panamax)
        var vesselDto = new VesselDto
        {
            Name = "Test Vessel",
            ImoNumber = "IMO 4569858",
            PhysicalCharacteristics = new PhysicalCharacteristicsDto
            {
                Length = 250,
                Depth = 12,
                Draft = 10
            },
            Type = new VesselTypeDto
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
            },
            Owner = new ShippingAgentOrganizationDto
            {
                Name = "Global Shipping Co.",
                AltNames = new List<string> { "TSA", "Test Agent" }.ToArray(),
                TaxNumber = "123456789",
                Address = new Address("123 Test St", "Test City", "Test Country", "12345"),
                Representatives = new List<RepresentativeDto>()
                {
                    new RepresentativeDto {
                        Name = "John Doe",
                        EmailAddress = "email",
                        Phone = "phone",
                        CitizenshipId = 123456789
                    }
                }
            }
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
