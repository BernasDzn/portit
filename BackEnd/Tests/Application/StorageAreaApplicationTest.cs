using Api.Application.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Api.Application;
using Api.Domain.Entities;
using System.Net;

namespace Tests.Application;

public class StorageAreaApplicationTest : WebApplicationFactory<Program>
{
	private static readonly string DatabaseName = $"TestDatabase_{Guid.NewGuid()}";
	private readonly HttpClient _client;

	public StorageAreaApplicationTest()
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
				context.Database.EnsureCreated();
				if (!context.StorageAreas.Any())
				{
					Bootstrap.Init(context, false);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error seeding database: {ex.Message}");
			}
		});
	}

	[Fact]
	public async Task GetAllStorageAreas_ReturnsStorageAreaList()
	{
		var response = await _client.GetAsync("/StorageArea");
		response.EnsureSuccessStatusCode();

		var storageAreas = await response.Content.ReadFromJsonAsync<List<StorageAreaDto>>();
		Assert.NotNull(storageAreas);
		Assert.NotEmpty(storageAreas);
	}

	[Fact]
	public async Task GetStorageAreaById_ReturnsStorageArea()
	{
		var testId = "YARD01";
		var response = await _client.GetAsync($"/StorageArea/{testId}");
		response.EnsureSuccessStatusCode();

		var storageArea = await response.Content.ReadFromJsonAsync<StorageAreaDto>();
		Assert.NotNull(storageArea);
		Assert.Equal(testId, storageArea.NameCode);
	}

	[Fact]
	public async Task GetStorageAreaById_ReturnsNotFound_OnNonExistentId()
	{
		var testId = "nonexistentid";
		var response = await _client.GetAsync($"/StorageArea/{testId}");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task AddStorageArea_ReturnsCreatedStorageArea()
	{
		var storageAreaDto = new CreateStorageAreaDto
		{
			NameCode = "NEWYARD",
			Location = "New Location",
			Type = StorageAreaType.Yard,
			Capacity = 100,
			CurrentOccupancy = 0,
			DockServices = new HashSet<CreateDockRelationDto>()
		};

		var response = await _client.PostAsJsonAsync("/StorageArea", storageAreaDto);
		response.EnsureSuccessStatusCode();
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var createdStorageArea = await response.Content.ReadFromJsonAsync<StorageAreaDto>();
		Assert.NotNull(createdStorageArea);
		Assert.Equal(storageAreaDto.NameCode, createdStorageArea.NameCode);
		Assert.Equal(storageAreaDto.Location, createdStorageArea.Location);
	}

	   [Fact]
	   public async Task AddStorageArea_ReturnsConflict_OnDuplicateIdCode()
	   {
		   // Ensure YARD01 exists first
		   var initialDto = new CreateStorageAreaDto
		   {
			   NameCode = "YARD01",
			   Location = "Initial Location",
			   Type = StorageAreaType.Yard,
			   Capacity = 100,
			   CurrentOccupancy = 0,
			   DockServices = new HashSet<CreateDockRelationDto>()
		   };
		   var initialResponse = await _client.PostAsJsonAsync("/StorageArea", initialDto);

		   // Now try to add duplicate
		   var storageAreaDto = new CreateStorageAreaDto
		   {
			   NameCode = "YARD01",
			   Location = "Duplicate Location",
			   Type = StorageAreaType.Yard,
			   Capacity = 100,
			   CurrentOccupancy = 0,
			   DockServices = new HashSet<CreateDockRelationDto>()
		   };
		   var response = await _client.PostAsJsonAsync("/StorageArea", storageAreaDto);
		   Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
	   }

	[Fact]
	public async Task AddStorageArea_ReturnsBadRequest_OnInvalidData()
	{
		var storageAreaDto = new CreateStorageAreaDto
		{
			NameCode = "",
			Location = "Invalid Location",
			Type = StorageAreaType.Yard,
			Capacity = 100,
			CurrentOccupancy = 0,
			DockServices = new HashSet<CreateDockRelationDto>()
		};

		var response = await _client.PostAsJsonAsync("/StorageArea", storageAreaDto);
		Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task AddStorageArea_ReturnsBadRequest_OnNullData()
	{
		CreateStorageAreaDto? storageAreaDto = null;

		var response = await _client.PostAsJsonAsync("/StorageArea", storageAreaDto);
		Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task UpdateStorageArea_ReturnsOk()
	{
		var storageAreaDto = new CreateStorageAreaDto
		{
			NameCode = "YARD01",
			Location = "Updated Location",
			Type = StorageAreaType.Yard,
			Capacity = 200,
			CurrentOccupancy = 10,
			DockServices = new HashSet<CreateDockRelationDto>()
		};

		var response = await _client.PutAsJsonAsync($"/StorageArea/{storageAreaDto.NameCode}", storageAreaDto);
		response.EnsureSuccessStatusCode();
		Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

		var updatedStorageArea = await response.Content.ReadFromJsonAsync<StorageAreaDto>();
		Assert.NotNull(updatedStorageArea);
		Assert.Equal(storageAreaDto.Location, updatedStorageArea.Location);
		Assert.Equal(storageAreaDto.Capacity, updatedStorageArea.Capacity);
	}

	[Fact]
	public async Task UpdateStorageArea_ReturnsNotFound_OnNonExistentId()
	{
		var storageAreaDto = new CreateStorageAreaDto
		{
			NameCode = "NONEXISTENT",
			Location = "Non Existent Location",
			Type = StorageAreaType.Yard,
			Capacity = 100,
			CurrentOccupancy = 0,
			DockServices = new HashSet<CreateDockRelationDto>()
		};

		var response = await _client.PutAsJsonAsync($"/StorageArea/{storageAreaDto.NameCode}", storageAreaDto);
		Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
	}

	   [Fact]
	   public async Task UpdateStorageArea_ReturnsBadRequest_OnInvalidData()
	   {
		   // Ensure YARD01 exists first
		   var initialDto = new CreateStorageAreaDto
		   {
			   NameCode = "YARD01",
			   Location = "Valid Location",
			   Type = StorageAreaType.Yard,
			   Capacity = 100,
			   CurrentOccupancy = 0,
			   DockServices = new HashSet<CreateDockRelationDto>()
		   };
		   var initialResponse = await _client.PostAsJsonAsync("/StorageArea", initialDto);
		   initialResponse.EnsureSuccessStatusCode();

		   // Now try to update with invalid data
		   var storageAreaDto = new CreateStorageAreaDto
		   {
			   NameCode = "YARD01",
			   Location = "",
			   Type = StorageAreaType.Yard,
			   Capacity = 100,
			   CurrentOccupancy = 0,
			   DockServices = new HashSet<CreateDockRelationDto>()
		   };
		   var response = await _client.PutAsJsonAsync($"/StorageArea/{storageAreaDto.NameCode}", storageAreaDto);
		   Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	   }

	[Fact]
	public async Task UpdateStorageArea_ReturnsBadRequest_OnNullData()
	{
		CreateStorageAreaDto? storageAreaDto = null;

		var response = await _client.PutAsJsonAsync($"/StorageArea/YARD01", storageAreaDto);
		Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
	}
}
