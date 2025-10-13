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

namespace Api.Tests.Controllers
{
    public class DockControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DockControllerTests(WebApplicationFactory<Program> factory)
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
        [InlineData("/Dock")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Get_ReturnData()
        {

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApiContext>();
                await context.Database.EnsureCreatedAsync();
            }


            var response = await _client.GetAsync("/Dock");

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var docks = JsonSerializer.Deserialize<List<DockDto>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(docks);
            Assert.IsType<List<DockDto>>(docks);
        }


        [Fact]
        public async Task Post_Valid_Dock_Successfully_Creates_Dock()
        {

            var dock = new
            {
                Name = "Dock Test",
                Location = "Location Test",
                PhysicalCharacteristics = new PhysicalCharacteristicsDto
                {
                    Length = 350,
                    Depth = 40,
                    Draft = 20
                },
                SupportedVesselTypes = new List<VesselTypeDto>
                {
                    new VesselTypeDto
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
                    }
                }
            };

            var response = await _client.PostAsJsonAsync("/Dock", dock);

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var createdDock = await response.Content.ReadFromJsonAsync<DockDto>();
            Assert.NotNull(createdDock);
            Assert.Equal(dock.Name, createdDock.Name);
            Assert.Equal(dock.Location, createdDock.Location);
            Assert.Equal(dock.PhysicalCharacteristics.Length, createdDock.PhysicalCharacteristics.Length);
            Assert.Equal(dock.PhysicalCharacteristics.Depth, createdDock.PhysicalCharacteristics.Depth);
            Assert.Equal(dock.PhysicalCharacteristics.Draft, createdDock.PhysicalCharacteristics.Draft);
            Assert.Equal(dock.SupportedVesselTypes.Count, createdDock.SupportedVesselTypes.Count);

            // Verify the dock was added to the collection
            var getResponse = await _client.GetAsync("/Dock");
            getResponse.EnsureSuccessStatusCode();
            var stringResponse = await getResponse.Content.ReadAsStringAsync();
            var docks = JsonSerializer.Deserialize<List<DockDto>>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(docks);
            Assert.Contains(docks, d => d.Name == createdDock.Name);
        }

        [Fact]
        public async Task Post_Invalid_Dock_Returns_BadRequest()
        {
            var invalidDock = new
            {
                // Missing Name
                Location = "Location Test",
                PhysicalCharacteristics = new PhysicalCharacteristics
                {
                    Length = 350,
                    Depth = 40,
                    Draft = 20
                },
                SupportedVesselTypes = new List<VesselTypeDto>()
            };

            var response = await _client.PostAsJsonAsync("/Dock", invalidDock);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_Valid_Dock_Successfully_Updates_Dock()
        {
            var updatedDock = new
            {
                Name = "Dock A",
                Location = "Updated Location",
                PhysicalCharacteristics = new PhysicalCharacteristics
                {
                    Length = 400,
                    Depth = 50,
                    Draft = 25
                },
                SupportedVesselTypes = new List<VesselTypeDto>
                {
                    new VesselTypeDto
                    {
                        Name = "Post-Panamax",
                        Description = "Larger than Panamax",
                        MaxNumberOfRows = 30,
                        MaxNumberOfBays = 15,
                        MaxNumberOfTiers = 7,
                        PhysicalCharacteristics = new PhysicalCharacteristicsDto
                        {
                            Length = 400,
                            Depth = 18,
                            Draft = 14
                        }
                    }
                }
            };

            var putResponse = await _client.PutAsJsonAsync("/Dock/Dock Test", updatedDock);
            putResponse.EnsureSuccessStatusCode();
            var returnedDock = await putResponse.Content.ReadFromJsonAsync<DockDto>();
            Assert.NotNull(returnedDock);
            Assert.Equal(updatedDock.Location, returnedDock.Location);
            Assert.Equal(updatedDock.PhysicalCharacteristics.Length, returnedDock.PhysicalCharacteristics.Length);
            Assert.Equal(updatedDock.PhysicalCharacteristics.Depth, returnedDock.PhysicalCharacteristics.Depth);
            Assert.Equal(updatedDock.PhysicalCharacteristics.Draft, returnedDock.PhysicalCharacteristics.Draft);

            // Verify the dock was updated in the collection
            var getResponse = await _client.GetAsync("/Dock");
            getResponse.EnsureSuccessStatusCode();
            var stringResponse = await getResponse.Content.ReadAsStringAsync();
            var docks = JsonSerializer.Deserialize<List<DockDto>>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(docks);
            var fetchedDock = docks.FirstOrDefault(d => d.Name == updatedDock.Name);
            Assert.NotNull(fetchedDock);
            Assert.Equal(updatedDock.Location, fetchedDock.Location);
            Assert.Equal(updatedDock.PhysicalCharacteristics.Length, fetchedDock.PhysicalCharacteristics.Length);
            Assert.Equal(updatedDock.PhysicalCharacteristics.Depth, fetchedDock.PhysicalCharacteristics.Depth);
            Assert.Equal(updatedDock.PhysicalCharacteristics.Draft, fetchedDock.PhysicalCharacteristics.Draft);
            Assert.Equal(updatedDock.SupportedVesselTypes.Count, fetchedDock.SupportedVesselTypes.Count);
            for (int i = 0; i < updatedDock.SupportedVesselTypes.Count; i++)
            {
                Assert.Equal(updatedDock.SupportedVesselTypes[i].Name, fetchedDock.SupportedVesselTypes[i].Name);
            }
        }

        [Fact]
        public async Task Put_Invalid_Dock_Returns_BadRequest()
        {
            var invalidUpdatedDock = new
            {
                //Missing Name and Location
                PhysicalCharacteristics = new PhysicalCharacteristics
                {
                    Length = 400,
                    Depth = 50,
                    Draft = 25
                },
                SupportedVesselTypes = new List<VesselTypeDto>() //Empty list should be invalid
            };

            var putResponse = await _client.PutAsJsonAsync($"/Dock/Dock A", invalidUpdatedDock);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, putResponse.StatusCode);
        }
    }
}