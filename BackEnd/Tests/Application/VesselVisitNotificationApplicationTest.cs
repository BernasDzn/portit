using Api.Application.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Api.Infrastructure.Utilities;

namespace Tests.Application;

public class VesselVisitNotificationApplicationTest : WebApplicationFactory<Program>
{
	private static readonly string DatabaseName = $"TestDatabase_VVN_{Guid.NewGuid()}";
	private readonly HttpClient _client;

	public VesselVisitNotificationApplicationTest()
	{
		_client = CreateClient();
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiContext>));
			if (descriptor != null)
				services.Remove(descriptor);

			services.AddDbContext<ApiContext>(options =>
			{
				options.UseInMemoryDatabase(DatabaseName);
			});
		});

		builder.UseEnvironment("Testing");

		builder.ConfigureServices(services =>
		{
			var sp = services.BuildServiceProvider();
			using var scope = sp.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<ApiContext>();

			try
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();

				// Bootstrap default data used across the application tests
				Api.Application.Bootstrap.Init(context, false);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error seeding database: {ex.Message}");
			}
		});
	}

	[Fact]
	public async Task GetAll_ReturnsList()
	{
		var response = await _client.GetAsync("/VesselVisitNotification");
		response.EnsureSuccessStatusCode();

		var list = await response.Content.ReadFromJsonAsync<IEnumerable<VesselVisitNotificationDto>>();
		Assert.NotNull(list);
		Assert.NotEmpty(list);
	}

	[Fact]
	public async Task GetDecisions_ReturnsList_ForExistingNotification()
	{
		// Build a known notification id from bootstrap (PORTO, seq 1)
		var id = $"{DateTime.UtcNow.Year}-PORTO-000001";
		var response = await _client.GetAsync($"/VesselVisitNotification/decisions?vesselVisitNotificationId={id}");
		response.EnsureSuccessStatusCode();

		var list = await response.Content.ReadFromJsonAsync<IEnumerable<NotificationDecisionDto>>();
		Assert.NotNull(list);
		Assert.NotEmpty(list);
	}

	[Fact]
	public async Task Create_ReturnsCreated_WhenValid()
	{
		var dto = new CreateVesselVisitNotificationDto
		{
			NotificationId = string.Empty,
			ExpectedArrival = DateTime.UtcNow.AddDays(20),
			ExpectedDeparture = DateTime.UtcNow.AddDays(22),
			IsCargoHazardous = false,
			VesselImoNumber = "IMO 7585229", // seeded in Bootstrap
			SubmitterId = 908029952u // seeded representative from Bootstrap
		};

		var response = await _client.PostAsJsonAsync("/VesselVisitNotification", dto);
		if (response.StatusCode != System.Net.HttpStatusCode.Created)
		{
			var text = await response.Content.ReadAsStringAsync();
			throw new Exception($"Unexpected response status {response.StatusCode}. Body: {text}");
		}

		var created = await response.Content.ReadFromJsonAsync<VesselVisitNotificationDto>();
		Assert.NotNull(created);
		Assert.Equal(dto.VesselImoNumber, created.Vessel.ImoNumber);
	}

	[Fact]
	public async Task Create_ReturnsNotFound_WhenVesselDoesNotExist()
	{
		var dto = new CreateVesselVisitNotificationDto
		{
			NotificationId = string.Empty,
			ExpectedArrival = DateTime.UtcNow.AddDays(5),
			ExpectedDeparture = DateTime.UtcNow.AddDays(6),
			IsCargoHazardous = false,
			VesselImoNumber = "NONEXISTENT_IMO",
			SubmitterId = 908029952u
		};

		var response = await _client.PostAsJsonAsync("/VesselVisitNotification", dto);
		if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
		{
			var text = await response.Content.ReadAsStringAsync();
			throw new Exception($"Unexpected response status {response.StatusCode}. Body: {text}");
		}
	}

	[Fact]
	public async Task Update_ReturnsNoContent_WhenExists()
	{
		// Create a fresh notification (should be in-progress) and then update it
		var createDto = new CreateVesselVisitNotificationDto
		{
			NotificationId = string.Empty,
			ExpectedArrival = DateTime.UtcNow.AddDays(30),
			ExpectedDeparture = DateTime.UtcNow.AddDays(32),
			IsCargoHazardous = false,
			VesselImoNumber = "IMO 3815389",
			// Use a representative that belongs to the vessel owner (see Bootstrap)
			SubmitterId = 995128061u
		};

		var createResp = await _client.PostAsJsonAsync("/VesselVisitNotification", createDto);
		if (createResp.StatusCode != System.Net.HttpStatusCode.Created)
		{
			var text = await createResp.Content.ReadAsStringAsync();
			throw new Exception($"Create for update test failed with status {createResp.StatusCode}. Body: {text}");
		}

		var created = await createResp.Content.ReadFromJsonAsync<VesselVisitNotificationDto>();
		Assert.NotNull(created);

		var id = created.NotificationId;

		// prepare update DTO
		var dto = new CreateVesselVisitNotificationDto
		{
			NotificationId = id,
			ExpectedArrival = DateTime.UtcNow.AddDays(35),
			ExpectedDeparture = DateTime.UtcNow.AddDays(37),
			IsCargoHazardous = false,
			VesselImoNumber = "IMO 3815389",
			SubmitterId = 319982093u
		};

		var response = await _client.PutAsJsonAsync($"/VesselVisitNotification/{id}", dto);
		if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
		{
			var text = await response.Content.ReadAsStringAsync();
			throw new Exception($"Unexpected response status {response.StatusCode}. Body: {text}");
		}
	}


    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsOkAndPagedData()
    {
        var requestUri = "/VesselVisitNotification/filter?SubmitterCitizeshipId=908029952";

        var response = await _client.GetAsync(requestUri);

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotNull(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsOkWithEmptyData_WhenNoMatches()
    {
        var requestUri = "/VesselVisitNotification/filter?SubmitterCitizeshipId=908029952&Status=0";

        var response = await _client.GetAsync(requestUri);

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsOkWithData_WhenMultipleFiltersApplied()
    {
        var requestUri = "/VesselVisitNotification/filter?SubmitterCitizeshipId=908029952&Status=1";

        var response = await _client.GetAsync(requestUri);

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsBadRequest_WhenInvalidNumber()
    {
        var requestUri = "/VesselVisitNotification/filter?SubmitterCitizeshipId=-1";

        var response = await _client.GetAsync(requestUri);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsInternalServerError_WhenNoSubmitterGiven()
    {
        var requestUri = "/VesselVisitNotification/filter";

        var response = await _client.GetAsync(requestUri);
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsPagedResults()
    {
        var requestUri = "/VesselVisitNotification/filter?SubmitterCitizeshipId=908029952&PageNumber=1&PageSize=0";

        var response = await _client.GetAsync(requestUri);

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsFiltered_WhenInvalidParameterPassed()
    {
        var requestUri = "/VesselVisitNotification/filter?SubmitterCitizeshipId=908029952&InvalidParam=xyz";

        var response = await _client.GetAsync(requestUri);

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<VesselVisitNotificationStatusDto>>();
        Assert.NotNull(pagedResult);
        Assert.NotEmpty(pagedResult.Items);
    }
}