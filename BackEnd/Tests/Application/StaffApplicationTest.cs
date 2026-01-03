using Api.Application.DataTransfer;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Api.Infrastructure.Utilities;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;

namespace Tests.Application;

public class StaffApplicationTest : BaseApplicationTest
{
    private readonly HttpClient _client;

    public StaffApplicationTest()
    {
        _client = Client;
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
        var response = await _client.GetAsync($"/Staff/filter?mechanograficNumber=STF250003");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<StaffDto>>();
        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
        Assert.Equal("STF250003", page.Items.First().MechanographicNumber);
    }

    [Fact]
    public async Task GetStaffById_ReturnsNotFound_OnNonExistent()
    {
        var response = await _client.GetAsync($"/Staff/filter?MechanographicNumber=AAAA");
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
            MechanographicNumber = "STF260001",
            Name = "Bob",
            Email = "bob@example.com",
            PhoneNumber = "900000002",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "STSOP" }
        };

        var response = await _client.PostAsJsonAsync("/Staff", createDto);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<StaffDto>();
        Assert.NotNull(created);
        Assert.StartsWith("STF26", created.MechanographicNumber);
    }

    [Fact]
    public async Task CreateStaff_ReturnsBadRequest_WhenQualificationMissing()
    {
        var createDto = new CreateStaffDto
        {
            MechanographicNumber = "STF250006",
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
            MechanographicNumber = "STF250001",
            Name = "Dup",
            Email = "dup@example.com",
            PhoneNumber = "900000009",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "STSOP" }
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
            QualificationsCodes = new List<string> { "STSOP" }
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
        var response = await _client.DeleteAsync("/Staff/STF250001");
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
            MechanographicNumber = "STF250002",
            Name = "Alice Updated",
            Email = "alice2@example.com",
            PhoneNumber = "900000010",
            Status = 0,
            OperationalWindow = OperationalWindow.FullWeek(),
            QualificationsCodes = new List<string> { "STSOP" }
        };

        var response = await _client.PutAsJsonAsync($"/Staff/STF250002", dto);
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
            QualificationsCodes = new List<string> { "STSOP" }
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
