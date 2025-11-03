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

public class StaffApplicationTest : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = $"TestDatabase_Staff_{Guid.NewGuid()}";
    private readonly HttpClient _client;

    public StaffApplicationTest()
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
            // seed database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApiContext>();
            var logger = scopedServices.GetRequiredService<ILogger<StaffApplicationTest>>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // seed qualifications used by staff
            var qualifications = new List<Qualification>
            {
                new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }),
                new Qualification(Guid.NewGuid(), new Code { Value = "Q2" }, new Designation { Value = "Qualification 2" })
            };
            db.Qualifications.AddRange(qualifications);

            // seed a staff using the in-memory list (avoid querying DB before SaveChanges)
            var qual = qualifications.First();
            var staff = new Staff(
                new StaffMechanographicNumber { Value = "MEC001" },
                new Designation { Value = "Alice" },
                new Email { Value = "alice@example.com" },
                new PhoneNumber { Value = "900000001" },
                OperationalWindow.FullWeek(),
                new List<Qualification> { qual }
            );
            db.Staffs.Add(staff);

            db.SaveChanges();
        });
    }

    [Fact]
    public async Task GetAllStaffs_ReturnsList()
    {
        var response = await _client.GetAsync("/Staff");
        response.EnsureSuccessStatusCode();

        var list = await response.Content.ReadFromJsonAsync<List<StaffDto>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task GetStaffById_ReturnsStaff_WhenExists()
    {
        var response = await _client.GetAsync($"/Staff/filter?mechanograficNumber=MEC001");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<StaffDto>>();
        Assert.NotNull(page);
        Assert.Single(page.Items);
        Assert.Equal("MEC001", page.Items.First().MechanographicNumber);
    }

    [Fact]
    public async Task GetStaffById_ReturnsNotFound_OnNonExistent()
    {
        var response = await _client.GetAsync($"/Staff/filter?mechanograficNumber=NONEXISTENT");
        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<Page<StaffDto>>();
        Assert.NotNull(page);
        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task CreateStaff_ReturnsCreated_WhenValid()
    {
        var createDto = new CreateStaffDto
        {
            MechanographicNumber = "MEC002",
            Name = "Bob",
            Email = "bob@example.com",
            PhoneNumber = "900000002",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "Q1" }
        };

        var response = await _client.PostAsJsonAsync("/Staff", createDto);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<StaffDto>();
        Assert.NotNull(created);
        Assert.Equal(createDto.MechanographicNumber, created.MechanographicNumber);
    }

    [Fact]
    public async Task CreateStaff_ReturnsBadRequest_WhenQualificationMissing()
    {
        var createDto = new CreateStaffDto
        {
            MechanographicNumber = "MEC003",
            Name = "Charlie",
            Email = "charlie@example.com",
            PhoneNumber = "900000003",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "NONEXIST" }
        };

        var response = await _client.PostAsJsonAsync("/Staff", createDto);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateStaff_ReturnsConflict_OnDuplicateMechanographicNumber()
    {
        var createDto = new CreateStaffDto
        {
            MechanographicNumber = "MEC001",
            Name = "Dup",
            Email = "dup@example.com",
            PhoneNumber = "900000009",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "Q1" }
        };

        var response = await _client.PostAsJsonAsync("/Staff", createDto);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateStaff_ReturnsBadRequest_OnInvalidData()
    {
        var createDto = new CreateStaffDto
        {
            MechanographicNumber = "",
            Name = "",
            Email = "not-an-email",
            PhoneNumber = "",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "Q1" }
        };

        var response = await _client.PostAsJsonAsync("/Staff", createDto);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStaff_ReturnsBadRequest_OnNullData()
    {
        CreateStaffDto? createDto = null;
        var response = await _client.PostAsJsonAsync("/Staff", createDto);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStaff_ReturnsBadRequest_OnWrongFormat()
    {
        var wrong = new { Wrong = "value" };
        var response = await _client.PostAsJsonAsync("/Staff", wrong);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_ReturnsOk_WhenExists()
    {
        var response = await _client.DeleteAsync("/Staff/MEC001");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_ReturnsBadRequest_WhenNotExists()
    {
        var response = await _client.DeleteAsync("/Staff/NONEXISTENT");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenExists()
    {
        var dto = new CreateStaffDto
        {
            MechanographicNumber = "MEC001",
            Name = "Alice Updated",
            Email = "alice2@example.com",
            PhoneNumber = "900000010",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "Q1" }
        };

        var response = await _client.PutAsJsonAsync($"/Staff/MEC001", dto);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenNonExistent()
    {
        var dto = new CreateStaffDto
        {
            MechanographicNumber = "NONEX",
            Name = "No",
            Email = "no@example.com",
            PhoneNumber = "900000099",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "Q1" }
        };

        var response = await _client.PutAsJsonAsync($"/Staff/NONEX", dto);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Filter_ReturnsPage()
    {
        var response = await _client.GetAsync("/Staff/filter?pageNumber=1&pageSize=10");
        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<Page<StaffDto>>();
        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
    }

}
