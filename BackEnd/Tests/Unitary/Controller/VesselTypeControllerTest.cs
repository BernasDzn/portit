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
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

namespace Tests.Unitary.Controller
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
                    PhysicalCharacteristics = null!
                });

            var result = await _controller.GetByName(testName);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<VesselTypeDto>(okResult.Value);
            Assert.Equal(testName, returnValue.Name);
        }

        [Fact]
        public async Task GetByName_ReturnsNotFound_WhenVesselDoesNotExist()
        {
            var vesselTypeName = "NONEXISTENT";
            _vesselTypeServiceMock.Setup(service => service.GetByName(It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Vessel type not found"));

            var result = await _controller.GetByName(vesselTypeName);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }


        [Fact]
        public async Task GetByName_ReturnsInternalServerError_OnException()
        {
            var vesselTypeName = "VesselType1";
            _vesselTypeServiceMock.Setup(service => service.GetByName(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.GetByName(vesselTypeName);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
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
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Add(It.IsAny<VesselTypeDto>()))
                .ReturnsAsync(newVesselType);

            var result = await _controller.Create(newVesselType);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<VesselTypeDto>(createdAtActionResult.Value);
            Assert.Equal(newVesselType.Name, returnValue.Name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenCreationFails()
        {
            var newVesselType = new VesselTypeDto
            {
                Name = "",
                Description = "A new vessel type",
                MaxNumberOfRows = 15,
                MaxNumberOfBays = 25,
                MaxNumberOfTiers = 6,
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Add(It.IsAny<VesselTypeDto>()))
                .ThrowsAsync(new ArgumentException("Could not create vessel type"));

            var result = await _controller.Create(newVesselType);

            var statusCodeResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenEntityAlreadyExists()
        {
            var newVesselType = new VesselTypeDto
            {
                Name = "Panamax",
                Description = "An existing vessel type",
                MaxNumberOfRows = 15,
                MaxNumberOfBays = 25,
                MaxNumberOfTiers = 6,
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Add(It.IsAny<VesselTypeDto>()))
                .ThrowsAsync(new EntityAlreadyExistsException("This vessel type already exists."));

            var result = await _controller.Create(newVesselType);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsInternalServerError_OnException()
        {
            var newVesselType = new VesselTypeDto
            {
                Name = "DCK004",
                Description = "Quaternary Dock",
                MaxNumberOfRows = 4,
                MaxNumberOfBays = 4,
                MaxNumberOfTiers = 4,
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Add(It.IsAny<VesselTypeDto>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Create(newVesselType);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
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
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselTypeDto>()))
                .ReturnsAsync(updatedVesselType);

            var result = await _controller.Update("OldVesselType", updatedVesselType);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<VesselTypeDto>(okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenInvalidDataIsProvided()
        {
            var updatedVesselType = new VesselTypeDto
            {
                Name = "",
                Description = "",
                MaxNumberOfRows = 0,
                MaxNumberOfBays = 0,
                MaxNumberOfTiers = 0,
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselTypeDto>()))
                .ThrowsAsync(new ArgumentException("Could not update vessel type"));

            var result = await _controller.Update("Panamax", updatedVesselType);

            var statusCodeResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenVesselTypeDoesNotExist()
        {
            var updatedVesselType = new VesselTypeDto
            {
                Name = "Updated Vessel Type",
                Description = "Updated Description",
                MaxNumberOfRows = 5,
                MaxNumberOfBays = 5,
                MaxNumberOfTiers = 5,
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselTypeDto>()))
                .ThrowsAsync(new EntityNotFoundException("Vessel type not found"));

            var result = await _controller.Update("NONEXISTENT", updatedVesselType);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_ReturnsInternalServerError_OnException()
        {
            var updatedVesselType = new VesselTypeDto
            {
                Name = "Updated Vessel Type",
                Description = "Updated Description",
                MaxNumberOfRows = 5,
                MaxNumberOfBays = 5,
                MaxNumberOfTiers = 5,
                PhysicalCharacteristics = null!
            };

            _vesselTypeServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselTypeDto>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Update("DCK001", updatedVesselType);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
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
        public async Task Filter_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var filter = new VesselTypeFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            _vesselTypeServiceMock.Setup(service => service.FilterVesselTypes(It.IsAny<VesselTypeFilter>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Filter(filter);

            var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        }

    }
}