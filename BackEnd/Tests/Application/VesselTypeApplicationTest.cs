using Api.Application.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Api.Infrastructure.Utilities;
using Api.Domain.ValueObjects;


namespace Tests.Application;

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
    public async Task AddVesselType_WithDuplicateName_ReturnsConflict()
    {
        var newVesselType = new VesselTypeDto
        {
            Name = "Panamax", // Existing name
            Description = "Max size for Panama Canal",
            MaxNumberOfRows = 30,
            MaxNumberOfBays = 15,
            MaxNumberOfTiers = 10,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 300,
                Depth = 25,
                Draft = 10
            }
        };

        var response = await _client.PostAsJsonAsync("/VesselType", newVesselType);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselType_WithInvalidData_ReturnsBadRequest()
    {
        var newVesselType = new VesselTypeDto
        {
            Name = "",
            Description = "Invalid Data Vessel Type",
            MaxNumberOfRows = 0,
            MaxNumberOfBays = 0,
            MaxNumberOfTiers = 0,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 100,
                Depth = 10,
                Draft = 5
            }
        };

        var response = await _client.PostAsJsonAsync("/VesselType", newVesselType);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselType_OnNullData_ReturnsBadRequest()
    {
        VesselTypeDto? newVesselType = null;

        var response = await _client.PostAsJsonAsync("/VesselType", newVesselType);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselType_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PostAsJsonAsync("/VesselType", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVesselType_ReturnsOk()
    {
        var updatedVesselType = new VesselTypeDto
        {
            Name = "Handymax",
            Description = "Updated description for Handymax vessel type.",
            MaxNumberOfRows = 32,
            MaxNumberOfBays = 16,
            MaxNumberOfTiers = 11,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 310,
                Depth = 26,
                Draft = 11
            }
        };

        var response = await _client.PutAsJsonAsync($"/VesselType/{updatedVesselType.Name}", updatedVesselType);

        response.EnsureSuccessStatusCode();

        // Verify the update persisted
        var getResponse = await _client.GetAsync($"/VesselType/{updatedVesselType.Name}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedVesselType = await getResponse.Content.ReadFromJsonAsync<VesselTypeDto>();
        Assert.NotNull(retrievedVesselType);
        Assert.Equal("Updated description for Handymax vessel type.", retrievedVesselType.Description);
    }

    [Fact]
    public async Task UpdateVesselType_NonExistingName_ReturnsNotFound()
    {
        var updatedVesselType = new VesselTypeDto
        {
            Name = "NONEXISTENT",
            Description = "Non-existing Vessel Type",
            MaxNumberOfRows = 500,
            MaxNumberOfBays = 35,
            MaxNumberOfTiers = 20,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 500,
                Depth = 35,
                Draft = 20
            }
        };

        var response = await _client.PutAsJsonAsync($"/VesselType/{updatedVesselType.Name}", updatedVesselType);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task UpdateVesselType_WithInvalidData_ReturnsBadRequest()
    {
        var updatedVesselType = new VesselTypeDto
        {
            Name = "",
            Description = "",
            MaxNumberOfRows = 0,
            MaxNumberOfBays = 0,
            MaxNumberOfTiers = 0,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 1,
                Depth = 1,
                Draft = 1
            }
        };

        var response = await _client.PutAsJsonAsync($"/VesselType/Panamax", updatedVesselType);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
    

    [Fact]
    public async Task UpdateVesselType_OnNullData_ReturnsBadRequest()
    {
        VesselTypeDto? updatedVesselType = null;

        var response = await _client.PutAsJsonAsync($"/VesselType/Panamax", updatedVesselType);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVesselType_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PutAsJsonAsync($"/VesselType/Panamax", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
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
    public async Task FilterVesselType_ReturnsPagedResult()
    {
        var response = await _client.GetAsync("/VesselType/filter?pageNumber=1&pageSize=2");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
        Assert.Equal(1, pagedResult.PageNumber);
        Assert.Equal(2, pagedResult.PageSize);
    }

    [Fact]
    public async Task FilterVesselType_WithNameFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselType/filter?Name=Panamax&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselType_WithDescriptionFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselType/filter?Description=Max%20size%20for%20Panama%20Canal&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }


    [Fact]
    public async Task FilterVesselType_WithMultipleFilters_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselType/filter?Name=Panamax&Description=Max%20size%20for%20Panama%20Canal&pageNumber=1&pageSize=5");


        response.EnsureSuccessStatusCode();
        var pagedDocks = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedDocks);
        Assert.NotEmpty(pagedDocks.Items);
    }

    [Fact]
    public async Task FilterVesselType_NoMatches_ReturnsEmptyResult()
    {
        var response = await _client.GetAsync("/VesselType/filter?Name=nonexistent&Description=nonexistent&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselType_PageNumberExceedsTotalPages_ReturnsEmptyResult()
    {
        var response = await _client.GetAsync("/VesselType/filter?pageNumber=10&pageSize=2");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselType_ReturnsFiltered_WhenInvalidParameterPassed()
    {
        var response = await _client.GetAsync("/VesselType/filter?invalidParam=someValue&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselTypeDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }
}