using Api.Application.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Api.Infrastructure.Utilities;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;

namespace Tests.Application;

public class PhysicalResourceApplicationTest : WebApplicationFactory<Program>
{
    private readonly string _databaseName;
    private readonly HttpClient _client;

    public PhysicalResourceApplicationTest()
    {
        _databaseName = $"TestDatabase_{Guid.NewGuid()}";
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
                options.UseInMemoryDatabase(_databaseName);
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
    public async Task GetAll_PhysicalResources_ReturnsOk()
    {
        var response = await _client.GetAsync("/PhysicalResource");

        response.EnsureSuccessStatusCode();
        var resources = await response.Content.ReadFromJsonAsync<IEnumerable<object>>();
        Assert.NotNull(resources);
        Assert.NotEmpty(resources);
        Assert.Equal(6, resources.Count());
    }

    [Fact]
    public async Task GetByCode_ExistingCode_ReturnsOk()
    {
        var response = await _client.GetAsync("/PhysicalResource/STS001");

        response.EnsureSuccessStatusCode();
        var resource = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(resource);
    }

    [Fact]
    public async Task GetByCode_NonExistingCode_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/PhysicalResource/AAAA");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Filter_PhysicalResources_ReturnsPagedResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?pageNumber=1&pageSize=2");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult.Items.Count());
        Assert.Equal(1, pagedResult.PageNumber);
        Assert.Equal(2, pagedResult.PageSize);
    }

    [Fact]
    public async Task Filter_PhysicalResources_WithCodeFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?Code=c&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult.Items.Count());
    }

    [Fact]
    public async Task Filter_PhysicalResources_WithDescriptionFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?Description=d&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult.Items.Count());
    }

    [Fact]
    public async Task Filter_PhysicalResources_WithTypeFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?type=2&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult.Items.Count());
    }

    [Fact]
    public async Task Filter_PhysicalResources_WithStatusFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?status=1&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult.Items.Count());
    }

    [Fact]
    public async Task Filter_PhysicalResources_WithMultipleFilters_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?Code=s&Description=s&Status=1&Type=0");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Single(pagedResult.Items);
    }

    [Fact]
    public async Task Filter_PhysicalResources_NoMatches_ReturnsEmptyResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?Code=nonexistent&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task Filter_PhysicalResources_PageNumberExceedsTotalPages_ReturnsEmptyResult()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?pageNumber=10&pageSize=2");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterQualification_ReturnsFiltered_WhenInvalidParameterPassed()
    {
        var response = await _client.GetAsync("/PhysicalResource/filter?invalidParam=someValue&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Equal(5, pagedResult.Items.Count());
    }

    [Fact]
    public async Task AddSTSCrane_ReturnsCreated()
    {
        var newCrane = new CreateSTSCraneDto
        {
            Code = "ZZZZ",
            Description = "ZZZZ",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string> { "STSOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 50,
            ServingDockCode = "DCK002"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddSTSCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var createdCrane = await response.Content.ReadFromJsonAsync<STSCraneDto>();
        Assert.NotNull(createdCrane);
        Assert.Equal("ZZZZ", createdCrane.Code);

        // Verify it can be retrieved
        var getResponse = await _client.GetAsync($"/PhysicalResource/{createdCrane.Code}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedCrane = await getResponse.Content.ReadFromJsonAsync<STSCraneDto>();
        Assert.NotNull(retrievedCrane);
        Assert.Equal(createdCrane.Code, retrievedCrane.Code);
    }

    [Fact]
    public async Task AddSTSCrane_WithNonExistingDock_ReturnsNotFound()
    {
        var newCrane = new CreateSTSCraneDto
        {
            Code = "YYYY",
            Description = "YYYY",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string> { "STSOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 50,
            ServingDockCode = "NONEXISTENT"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddSTSCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddSTSCrane_WithDuplicateCode_ReturnsConflict()
    {
        var newCrane = new CreateSTSCraneDto
        {
            Code = "STS001", // Existing code
            Description = "Duplicate Code Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string> { "STSOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 50,
            ServingDockCode = "DCK002"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddSTSCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddSTSCrane_WithInvalidData_ReturnsBadRequest()
    {
        var newCrane = new CreateSTSCraneDto
        {
            Code = "",
            Description = "Invalid Data Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string> { "STSOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 50,
            ServingDockCode = "DCK002"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddSTSCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddSTSCrane_MissingQualification_ReturnsNotFound()
    {
        var newCrane = new CreateSTSCraneDto
        {
            Code = "WWWW",
            Description = "WWWW",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string> { "NONEXISTENT" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 50,
            ServingDockCode = "DCK002"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddSTSCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddSTSCrane_OnNullData_ReturnsBadRequest()
    {
        CreateSTSCraneDto? newCrane = null;

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddSTSCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddSTSCrane_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddSTSCrane", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddYardCrane_ReturnsCreated()
    {
        var newCrane = new CreateYardCraneDto
        {
            Code = "QQQQQ",
            Description = "QQQQQ",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "YACOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            LiftingCapacity = 40
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddYardCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var createdCrane = await response.Content.ReadFromJsonAsync<YardCraneDto>();
        Assert.NotNull(createdCrane);
        Assert.Equal("QQQQQ", createdCrane.Code);

        // Verify it can be retrieved
        var getResponse = await _client.GetAsync($"/PhysicalResource/{createdCrane.Code}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedCrane = await getResponse.Content.ReadFromJsonAsync<YardCraneDto>();
        Assert.NotNull(retrievedCrane);
        Assert.Equal(createdCrane.Code, retrievedCrane.Code);
    }

    [Fact]
    public async Task AddYardCrane_WithDuplicateCode_ReturnsConflict()
    {
        var newCrane = new CreateYardCraneDto
        {
            Code = "YC001",
            Description = "Duplicate Code Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "YACOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            LiftingCapacity = 40
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddYardCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddYardCrane_WithInvalidData_ReturnsBadRequest()
    {
        var newCrane = new CreateYardCraneDto
        {
            Code = "",
            Description = "Invalid Data Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "YACOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            LiftingCapacity = 40
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddYardCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddYardCrane_MissingQualification_ReturnsNotFound()
    {
        var newCrane = new CreateYardCraneDto
        {
            Code = "RRRRR",
            Description = "RRRRR",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "NONEXISTENT" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            LiftingCapacity = 40
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddYardCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddYardCrane_OnNullData_ReturnsBadRequest()
    {
        CreateYardCraneDto? newCrane = null;

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddYardCrane", newCrane);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddYardCrane_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddYardCrane", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddTruck_ReturnsCreated()
    {
        var newTruck = new CreateTruckDto
        {
            Code = "XXXXX",
            Description = "XXXXX",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 5,
            QualificationsCodes = new List<string> { "TRKDR" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 10,
            AverageSpeed = 60,
            MaxLoadCapacity = 20000
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddTruck", newTruck);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var createdTruck = await response.Content.ReadFromJsonAsync<TruckDto>();
        Assert.NotNull(createdTruck);
        Assert.Equal("XXXXX", createdTruck.Code);

        // Verify it can be retrieved
        var getResponse = await _client.GetAsync($"/PhysicalResource/{createdTruck.Code}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedTruck = await getResponse.Content.ReadFromJsonAsync<TruckDto>();
        Assert.NotNull(retrievedTruck);
        Assert.Equal(createdTruck.Code, retrievedTruck.Code);
    }

    [Fact]
    public async Task AddTruck_WithDuplicateCode_ReturnsConflict()
    {
        var newTruck = new CreateTruckDto
        {
            Code = "TRK001", // Existing code
            Description = "Duplicate Code Truck",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 5,
            QualificationsCodes = new List<string> { "TRKDR" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 10,
            AverageSpeed = 60,
            MaxLoadCapacity = 20000
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddTruck", newTruck);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddTruck_WithInvalidData_ReturnsBadRequest()
    {
        var newTruck = new CreateTruckDto
        {
            Code = "",
            Description = "",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 5,
            QualificationsCodes = new List<string> { "TRKDR" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 10,
            AverageSpeed = 60,
            MaxLoadCapacity = 20000
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddTruck", newTruck);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddTruck_MissingQualification_ReturnsNotFound()
    {
        var newTruck = new CreateTruckDto
        {
            Code = "WWWWW",
            Description = "WWWWW",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 5,
            QualificationsCodes = new List<string> { "NONEXISTENT" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 10,
            AverageSpeed = 60,
            MaxLoadCapacity = 20000
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddTruck", newTruck);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddTruck_OnNullData_ReturnsBadRequest()
    {
        CreateTruckDto? newTruck = null;

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddTruck", newTruck);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddTruck_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PostAsJsonAsync("/PhysicalResource/AddTruck", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSTSCrane_ReturnsOk()
    {
        var updatedCrane = new CreateSTSCraneDto
        {
            Code = "STS001",
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string> { "STSOP" },
            OperationalWindow = new OperationalWindow
            {
                Shifts = new List<OperationalWindow.Shift>() {}
            },
            ContainersPerHour = 35,
            LiftingCapacity = 20,
            ServingDockCode = "DCK002"
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateSTSCrane/{updatedCrane.Code}", updatedCrane);

        response.EnsureSuccessStatusCode();

        // Verify the update persisted
        var getResponse = await _client.GetAsync($"/PhysicalResource/STS001");
        getResponse.EnsureSuccessStatusCode();
        var retrievedCrane = await getResponse.Content.ReadFromJsonAsync<STSCraneDto>();
        Assert.NotNull(retrievedCrane);
        Assert.Equal("Updated STS Crane", retrievedCrane.Description);
        Assert.Equal(ResourceStatus.Maintenance, retrievedCrane.Status);
    }

    [Fact]
    public async Task UpdateSTSCrane_NonExistingCode_ReturnsNotFound()
    {
        var updatedCrane = new CreateSTSCraneDto
        {
            Code = "NONEXISTENT",
            Description = "Non-existing STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string> { "STSOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 55,
            ServingDockCode = "DCK001"
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateSTSCrane/{updatedCrane.Code}", updatedCrane);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSTSCrane_WithInvalidData_ReturnsBadRequest()
    {
        var updatedCrane = new CreateSTSCraneDto
        {
            Code = "",
            Description = "",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string> { "STSOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 55,
            ServingDockCode = "DCK001"
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateSTSCrane/STS001", updatedCrane);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSTSCrane_MissingQualification_ReturnsNotFound()
    {
        var updatedCrane = new CreateSTSCraneDto
        {
            Code = "STS001",
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string> { "NONEXISTENT" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 55,
            ServingDockCode = "DCK001"
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateSTSCrane/{updatedCrane.Code}", updatedCrane);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSTSCrane_OnNullData_ReturnsBadRequest()
    {
        CreateSTSCraneDto? updatedCrane = null;

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateSTSCrane/STS001", updatedCrane);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSTSCrane_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateSTSCrane/STS001", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateYard_ReturnsOk()
    {
        var updatedYard = new CreateYardCraneDto
        {
            Code = "YC001",
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string> { "YACOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            LiftingCapacity = 45
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateYardCrane/{updatedYard.Code}", updatedYard);
        response.EnsureSuccessStatusCode();

        // Verify the update persisted
        var getResponse = await _client.GetAsync($"/PhysicalResource/YC001");
        getResponse.EnsureSuccessStatusCode();
        var retrievedYard = await getResponse.Content.ReadFromJsonAsync<YardCraneDto>();
        Assert.NotNull(retrievedYard);
        Assert.Equal("Updated Yard Crane", retrievedYard.Description);
        Assert.Equal(ResourceStatus.Maintenance, retrievedYard.Status);
    }

    [Fact]
    public async Task UpdateYard_NonExistingCode_ReturnsNotFound()
    {
        var updatedYard = new CreateYardCraneDto
        {
            Code = "NONEXISTENT",
            Description = "Non-existing Yard Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string> { "YACOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            LiftingCapacity = 45
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateYardCrane/{updatedYard.Code}", updatedYard);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateYard_WithInvalidData_ReturnsBadRequest()
    {
        var updatedYard = new CreateYardCraneDto
        {
            Code = "",
            Description = "",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string> { "YACOP" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            LiftingCapacity = 45
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateYardCrane/YC001", updatedYard);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateYard_MissingQualification_ReturnsNotFound()
    {
        var updatedYard = new CreateYardCraneDto
        {
            Code = "YC001",
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string> { "NONEXISTENT" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            LiftingCapacity = 45
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateYardCrane/{updatedYard.Code}", updatedYard);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateYard_OnNullData_ReturnsBadRequest()
    {
        CreateYardCraneDto? updatedYard = null;

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateYardCrane/YC001", updatedYard);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateYard_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateYardCrane/YC001", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTruck_ReturnsOk()
    {
        var updatedTruck = new CreateTruckDto
        {
            Code = "TRK002",
            Description = "Updated Truck",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "TRKDR" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 15,
            AverageSpeed = 70,
            MaxLoadCapacity = 25000
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateTruck/TRK002", updatedTruck);

        response.EnsureSuccessStatusCode();

        // Verify the update persisted
        var getResponse = await _client.GetAsync($"/PhysicalResource/TRK002");
        getResponse.EnsureSuccessStatusCode();
        var retrievedTruck = await getResponse.Content.ReadFromJsonAsync<TruckDto>();
        Assert.NotNull(retrievedTruck);
        Assert.Equal("Updated Truck", retrievedTruck.Description);
        Assert.Equal(ResourceStatus.Maintenance, retrievedTruck.Status);
    }

    [Fact]
    public async Task UpdateTruck_NonExistingCode_ReturnsNotFound()
    {
        var updatedTruck = new CreateTruckDto
        {
            Code = "NONEXISTENT",
            Description = "Non-existing Truck",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "TRKDR" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 15,
            AverageSpeed = 70,
            MaxLoadCapacity = 25000
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateTruck/{updatedTruck.Code}", updatedTruck);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTruck_WithInvalidData_ReturnsBadRequest()
    {
        var updatedTruck = new CreateTruckDto
        {
            Code = "",
            Description = "",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "TRKDR" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 15,
            AverageSpeed = 70,
            MaxLoadCapacity = 25000
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateTruck/TRK001", updatedTruck);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTruck_MissingQualification_ReturnsNotFound()
    {
        var updatedTruck = new CreateTruckDto
        {
            Code = "TRK001",
            Description = "Updated Truck",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "NONEXISTENT" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 15,
            AverageSpeed = 70,
            MaxLoadCapacity = 25000
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateTruck/{updatedTruck.Code}", updatedTruck);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTruck_OnNullData_ReturnsBadRequest()
    {
        CreateTruckDto? updatedTruck = null;

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateTruck/TRK001", updatedTruck);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTruck_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PutAsJsonAsync($"/PhysicalResource/UpdateTruck/TRK001", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeactivatePhysicalResource_ReturnsOk()
    {
        var response = await _client.DeleteAsync("/PhysicalResource/STS002");
        response.EnsureSuccessStatusCode();

        // Verify the status persisted
        var getResponse = await _client.GetAsync($"/PhysicalResource/STS002");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeactivatePhysicalResource_NonExistingCode_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/PhysicalResource/NONEXISTENT");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}