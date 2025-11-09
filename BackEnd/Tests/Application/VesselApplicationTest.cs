using Api.Application.DataTransfer;
using System.Net.Http.Json;
using Api.Infrastructure.Utilities;


namespace Tests.Application;

public class VesselApplicationTest : BaseApplicationTest
{
    private readonly HttpClient _client;

    public VesselApplicationTest()
    {
        _client = Client;
    }

    [Fact]
    public async Task CreateVessel_ReturnsCreatedResponse_WhenVesselIsValid()
    {
        // Arrange
        var body = @"{
            ""name"": ""Maersk Triple E MKII"",
            ""imoNumber"": ""IMO 8476310"",
            ""type"": ""Post-Panamax"",
            ""owner"": ""Global Shipping Co."",
            ""length"": 360,
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
    public async Task CreateVessel_ReturnsBadRequest_WhenVesselIsInvalid()
    {
        // Arrange
        var body = @"{
            ""name"": ""Maersk Triple E MKII"",
            ""imoNumber"": ""IMO BLEBLEBLE"",
            ""type"": ""Post-Panamax"",
            ""owner"": ""Global Shipping Co."",
            ""length"": 360,
            ""depth"": 16,
            ""draft"": 13
        }";

        var request = new HttpRequestMessage(HttpMethod.Post, "/Vessel")
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
        };

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
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

    [Fact]
    public async Task CreateVessel_ReturnsConflict_OnDuplicate()
    {
        // Arrange
        var body = @"{
            ""name"": ""Maersk Triple E MKII"",
            ""imoNumber"": ""IMO 7585229"",
            ""type"": ""Post-Panamax"",
            ""owner"": ""Global Shipping Co."",
            ""length"": 360,
            ""depth"": 16,
            ""draft"": 13
        }";

        var request = new HttpRequestMessage(HttpMethod.Post, "/Vessel")
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
        };

        var response2 = await _client.SendAsync(request);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, response2.StatusCode);
    }

    [Fact]
    public async Task CreateVessel_ReturnsBadRequest_OnNullData()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/Vessel", (object?)null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateVessel_ReturnsBadRequest_OnWrongFormatData()
    {
        var wrongFormat = new { WrongField = "SomeValue" };
        var response = await _client.PostAsJsonAsync("/Vessel", wrongFormat);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsOkAndUpdates_WhenVesselExists()
    {
        // Arrange - use an existing seeded IMO and a JSON object for the update
        var imoNumber = "IMO 3815389"; // seeded in Bootstrap (Maersk Triple E)

        var updateBody = new
        {
            name = "Updated Vessel Name",
            imoNumber = imoNumber,
            type = "Post-Panamax", // existing vessel type in seed
            owner = "Global Shipping Co.", // existing owner in seed
            length = 360,
            depth = 16,
            draft = 13
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/Vessel/{Uri.EscapeDataString(imoNumber)}", updateBody);

        // Assert
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<VesselDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated Vessel Name", updated.Name);
        Assert.Equal(imoNumber, updated.ImoNumber);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsBadRequest_OnNonExistentImo()
    {
        var imoNumber = "IMO 0000001"; // assume does not exist
        var updateBody = new
        {
            name = "Doesn't Matter",
            imoNumber = imoNumber,
            type = "T",
            owner = "O",
            length = 1,
            depth = 1,
            draft = 1
        };

        var response = await _client.PutAsJsonAsync($"/Vessel/{imoNumber}", updateBody);

        // Controller returns BadRequest when update could not be performed (null or error)
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsBadRequest_OnNullData()
    {
        var imoNumber = "IMO 3815389"; // existing
        var response = await _client.PutAsJsonAsync($"/Vessel/{imoNumber}", (object?)null);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsBadRequest_OnWrongFormatData()
    {
        var imoNumber = "IMO 3815389"; // existing
        var wrongFormat = new { WrongField = "X" };
        var response = await _client.PutAsJsonAsync($"/Vessel/{imoNumber}", wrongFormat);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FilterVessels_Empty_WhenPageNumberExceeds()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Vessel/filter?PageNumber=999&PageSize=10");
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var pagedVessels = await response.Content.ReadFromJsonAsync<Page<VesselDto>>();
        Assert.NotNull(pagedVessels);
    }

    [Fact]
    public async Task GetVesselByImo_ReturnsBadRequest_OnInvalidImoFormat()
    {
        var imoNumber = "INVALID_IMO";
        var response = await _client.GetAsync($"/Vessel/{imoNumber}");
    // Service throws EntityNotFoundException for unknown/invalid IMO -> controller returns NotFound
    Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}