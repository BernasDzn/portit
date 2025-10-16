using Xunit;
using Api.Application.Controllers;
using Api.Application.Services;
using Api.Application.DataTransfer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Infrastructure.Persistence.Repositories;
using Api.Infrastructure.Persistence;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.ValueObjects;

namespace Tests.Unitary.Controllers
{
    public class VesselTypeControllerTests
    {
        private readonly Mock<IVesselTypeService> _vesselTypeServiceMock;
        private readonly VesselTypeController _controller;

        public VesselTypeControllerTests()
        {
            _vesselTypeServiceMock = new Mock<IVesselTypeService>();
            _controller = new VesselTypeController(_vesselTypeServiceMock.Object, new Mock<ILogger<VesselTypeController>>().Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfVesselTypes()
        {
            _vesselTypeServiceMock.Setup(service => service.GetVesselTypes())
                .ReturnsAsync(new List<VesselTypeDto>());

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<VesselTypeDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetByName_ReturnsOkResult_WithVesselType()
        {
            var testName = "Panamax";

            _vesselTypeServiceMock.Setup(service => service.GetByName(It.IsAny<string>()))
                .ReturnsAsync((string name) => new VesselTypeDto
                {
                    Name = name,
                    Description = $"A {name} description",
                    MaxNumberOfRows = 10,
                    MaxNumberOfBays = 20,
                    MaxNumberOfTiers = 5,
                    PhysicalCharacteristics = null
                });

            var result = await _controller.GetByName(testName);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<VesselTypeDto>(okResult.Value);
            Assert.Equal(testName, returnValue.Name);
        }

        [Fact]
        public async Task GetByName_ReturnsNotFound_WhenVesselDoesNotExist()
        {
            _vesselTypeServiceMock.Setup(service => service.GetByName(It.IsAny<string>()))
                .ReturnsAsync((VesselTypeDto?)null);

            var result = await _controller.GetByName("non-existent-name");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No Vessel type found with name: non-existent-name", notFoundResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenVesselTypeIsCreated()
        {
            var newVesselType = new VesselTypeDto
            {
                Name = "NewType",
                Description = "A new vessel type",
                MaxNumberOfRows = 15,
                MaxNumberOfBays = 25,
                MaxNumberOfTiers = 6,
                PhysicalCharacteristics = null
            };

            _vesselTypeServiceMock.Setup(service => service.Add(It.IsAny<VesselTypeDto>()))
                .ReturnsAsync(newVesselType);

            var result = await _controller.Create(newVesselType);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<VesselTypeDto>(createdAtActionResult.Value);
            Assert.Equal(newVesselType.Name, returnValue.Name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenVesselTypeCreationFails()
        {
            var newVesselType = new VesselTypeDto
            {
                Name = "NewType",
                Description = "A new vessel type",
                MaxNumberOfRows = 15,
                MaxNumberOfBays = 25,
                MaxNumberOfTiers = 6,
                PhysicalCharacteristics = null
            };

            _vesselTypeServiceMock.Setup(service => service.Add(It.IsAny<VesselTypeDto>()))
                .ReturnsAsync((VesselTypeDto?)null);

            var result = await _controller.Create(newVesselType);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Cannot create vessel type", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenVesselTypeIsUpdated()
        {
            var updatedVesselType = new VesselTypeDto
            {
                Name = "UpdatedType",
                Description = "An updated vessel type",
                MaxNumberOfRows = 20,
                MaxNumberOfBays = 30,
                MaxNumberOfTiers = 7,
                PhysicalCharacteristics = null
            };

            _vesselTypeServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselTypeDto>()))
                .ReturnsAsync(updatedVesselType);

            var result = await _controller.Update("OldVesselType", updatedVesselType);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<VesselTypeDto>(okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenVesselTypeUpdateFails()
        {
            var updatedVesselType = new VesselTypeDto
            {
                Name = "UpdatedType",
                Description = "An updated vessel type",
                MaxNumberOfRows = 20,
                MaxNumberOfBays = 30,
                MaxNumberOfTiers = 7,
                PhysicalCharacteristics = null
            };

            _vesselTypeServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselTypeDto>()))
                .ReturnsAsync((VesselTypeDto?)null);

            var result = await _controller.Update("OldVesselType", updatedVesselType);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Could not update vessel type", badRequestResult.Value);
        }


        [Fact]
        public async Task Filter_ReturnsOkResult_WithPagedVesselTypes()
        {
            var filter = new VesselTypeFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            var pagedResult = new Page<VesselTypeDto>
            {
                Items = new List<VesselTypeDto>(),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            _vesselTypeServiceMock.Setup(service => service.FilterVesselTypes(It.IsAny<VesselTypeFilter>()))
                .ReturnsAsync(pagedResult);

            var result = await _controller.Filter(filter);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Page<VesselTypeDto>>(okResult.Value);
            Assert.Empty(returnValue.Items);
            Assert.Equal(filter.PageNumber, returnValue.PageNumber);
            Assert.Equal(filter.PageSize, returnValue.PageSize);
        }

        [Fact]
        public async Task Filter_ReturnsNotFound_WhenExceptionIsThrown()
        {
            var filter = new VesselTypeFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            _vesselTypeServiceMock.Setup(service => service.FilterVesselTypes(It.IsAny<VesselTypeFilter>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Filter(filter);

            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

    }
}