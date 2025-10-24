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

public class VesselVisitNotificationApplicationTest : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = $"TestDatabase_{Guid.NewGuid()}";
    private readonly HttpClient _client;

    public VesselVisitNotificationApplicationTest()
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
    public async Task AddVesselVisitNotification_ReturnsCreatedResponse_WhenIsValid()
    {

        var body = @"{
    ""NotificationId"": ""2025-PORTO-000006"",
    ""ExpectedArrival"": ""2024-10-01T10:00:00Z"",
    ""ExpectedDeparture"": ""2024-10-05T18:00:00Z"",
    ""IsCargoHazardous"": false,
    ""SpecialRequirements"": ""None"",
    ""CrewDetails"": {
        ""Captain"": {
            ""Value"": ""Captain John Doe""
        },
        ""TotalCrewMembers"": 25,
        ""SafetyOfficers"": [
            {
                ""CitizenID"": ""CITIZEN001"",
                ""Name"": ""Officer A"",
                ""Nationality"": ""PT""
            },
            {
                ""CitizenID"": ""CITIZEN002"",
                ""Name"": ""Officer B"",
                ""Nationality"": ""PT""
            }
        ]
        },
        ""LoadCargoManifest"": [
        {
            ""Position"": {
                ""Bay"": ""05"",
                ""Row"": ""BB"",
                ""Tier"": ""02""
            },
            ""StorageAreaCode"": ""WH1"",
            ""Container"": {
                ""ContainerNumber"": ""ABCD1234560"",
                ""CargoType"": 2,
                ""Description"": ""Electronics""
            }
        }
        ],
        ""UnloadCargoManifest"": [
        {
            ""Position"": {
                ""Bay"": ""10"",
                ""Row"": ""CC"",
                ""Tier"": ""03""
            },
            ""StorageAreaCode"": ""WH2"",
            ""Container"": {
                ""ContainerNumber"": ""CMAU2468103"",
                ""CargoType"": 1,
                ""Description"": ""General Consumer Products""
            }
        }
        ],
        ""VesselImoNumber"": ""IMO 7585229"",
            ""SubmitterId"": 908029952
        }";

        var request = new HttpRequestMessage(HttpMethod.Post, "/VesselVisitNotification")
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
        var createdVesselVisitNotification = await response.Content.ReadFromJsonAsync<VesselVisitNotificationDto>();
        Assert.NotNull(createdVesselVisitNotification);
    }

    [Fact]
    public async Task AddVesselVisitNotification_WithDuplicateCode_ReturnsConflict()
    {
        var newVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000001",
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952
        };

        var response = await _client.PostAsJsonAsync("/VesselVisitNotification", newVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselVisitNotification_WithInvalidData_ReturnsBadRequest()
    {
        var newVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952
        };

        var response = await _client.PostAsJsonAsync("/VesselVisitNotification", newVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselVisitNotification_MissingVessel_ReturnsNotFound()
    {
        var newVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000007",
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 0000000",
            SubmitterId = 908029952
        };

        var response = await _client.PostAsJsonAsync("/VesselVisitNotification", newVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselVisitNotification_MissingRepresentative_ReturnsNotFound()
    {
        var newVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000008",
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 900000000
        };

        var response = await _client.PostAsJsonAsync("/VesselVisitNotification", newVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselVisitNotification_OnNullData_ReturnsBadRequest()
    {
        CreateVesselVisitNotificationDto? newVesselVisitNotification = null;

        var response = await _client.PostAsJsonAsync("/VesselVisitNotification", newVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddVesselVisitNotification_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PostAsJsonAsync("/VesselVisitNotification", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsNoContent()
    {
        var updatedVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000005",
            ExpectedArrival = DateTime.Parse("2025-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2025-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952
        };

        var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/{updatedVesselVisitNotification.NotificationId}", updatedVesselVisitNotification);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }


    [Fact]
    public async Task UpdateVesselVisitNotification_NonExistingId_ReturnsNotFound()
    {
        var updatedVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = "NONEXISTENT",
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952
        };

        var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/{updatedVesselVisitNotification.NotificationId}", updatedVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_WithInvalidData_ReturnsBadRequest()
    {
        var updatedVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952
        };

        var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/{updatedVesselVisitNotification.NotificationId}", updatedVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task UpdateVesselVisitNotification_MissingVessel_ReturnsNotFound()
    {
        var updatedVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000008",
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 0000000",
            SubmitterId = 908029952
        };

        var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/{updatedVesselVisitNotification.NotificationId}", updatedVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_MissingRepresentative_ReturnsNotFound()
    {
        var updatedVesselVisitNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000008",
            ExpectedArrival = DateTime.Parse("2024-10-01T10:00:00Z"),
            ExpectedDeparture = DateTime.Parse("2024-10-05T18:00:00Z"),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 900000000
        };

        var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/{updatedVesselVisitNotification.NotificationId}", updatedVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task UpdateVesselVisitNotification_OnNullData_ReturnsBadRequest()
    {
        CreateVesselVisitNotificationDto? updatedVesselVisitNotification = null;

        var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/2025-PORTO-000001", updatedVesselVisitNotification);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_OnWrongFormat_ReturnsBadRequest()
    {
        var wrongFormatData = new
        {
            InvalidField = "InvalidValue"
        };

        var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/2025-PORTO-000001", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetVesselVisitNotifications_ReturnsOkResponse_WithListOfNotifications()
    {

        var request = new HttpRequestMessage(HttpMethod.Get, "/VesselVisitNotification");


        var response = await _client.SendAsync(request);


        response.EnsureSuccessStatusCode();
        var notifications = await response.Content.ReadFromJsonAsync<IEnumerable<VesselVisitNotificationDto>>();
        Assert.NotNull(notifications);
        Assert.NotEmpty(notifications);
    }

    [Fact]
    public async Task GetVesselVisitNotificationById_ReturnsOkResponse_WhenNotificationExists()
    {

        var code = "2025-PORTO-000001";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/VesselVisitNotification/{code}");


        var response = await _client.SendAsync(request);


        response.EnsureSuccessStatusCode();
        var notification = await response.Content.ReadFromJsonAsync<VesselVisitNotificationDto>();
        Assert.NotNull(notification);
        Assert.Equal(code, notification.NotificationId);
    }

    [Fact]
    public async Task GetVesselVisitNotificationById_ReturnsNotFoundResponse_WhenNotificationDoesNotExist()
    {

        var code = "2024-ABCD-000000"; // Non-existing Notification code
        var request = new HttpRequestMessage(HttpMethod.Get, $"/VesselVisitNotification/{code}");


        var response = await _client.SendAsync(request);


        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_ReturnsPagedResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&pageNumber=1&pageSize=2");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
        Assert.Equal(1, pagedResult.PageNumber);
        Assert.Equal(2, pagedResult.PageSize);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_WithStatusFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&Status=1&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_WithReasonFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&WithReason=false&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_WithDockAssignedFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&WithDockAssigned=false&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_WithVesselFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&Vessel=IMO%202345678&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_WithExpectedArrivalFromFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&ExpectedArrivalFrom=2025-01-01&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_WithExpectedArrivalToFilter_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&ExpectedArrivalTo=2026-01-01&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_WithMultipleFilters_ReturnsFilteredResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&Status=1&WithReason=false&WithDockAssigned=false&ExpectedArrivalFrom=2025-01-01&ExpectedArrivalTo=2026-01-01&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }


    [Fact]
    public async Task FilterVesselVisitNotification_NoMatches_ReturnsEmptyResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&Status=3&WithReason=false&WithDockAssigned=false&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_PageNumberExceedsTotalPages_ReturnsEmptyResult()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&pageNumber=10&pageSize=2");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotification_ReturnsFiltered_WhenInvalidParameterPassed()
    {
        var response = await _client.GetAsync("/VesselVisitNotification/filter?SubmitterCitizenshipId=908029952&invalidParam=someValue&pageNumber=1&pageSize=5");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }
}