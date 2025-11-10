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
using NuGet.Protocol;
using Moq;
using Api.Application.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;
using Api.Application.Services;

namespace Tests.Unitary.Controller
{
    public class DockControllerTest
    {

        private readonly Mock<IDockService> _dockServiceMock;
        private readonly DockController _controller;

        public DockControllerTest()
        {
            _dockServiceMock = new Mock<IDockService>();
            _controller = new DockController(_dockServiceMock.Object, new Mock<ILogger<DockController>>().Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfDocks()
        {
            _dockServiceMock.Setup(service => service.GetDocks())
                .ReturnsAsync(new List<DockDto>());

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<DockDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetByCode_ReturnsOkResult_WithDock()
        {
            var dockCode = "DCK001";
            _dockServiceMock.Setup(service => service.GetByCode(It.IsAny<string>()))
                .ReturnsAsync(new DockDto
                {
                    Code = dockCode,
                    Name = "Main Dock",
                    Location = "Harbor A",
                    PhysicalCharacteristics = null!,
                    SupportedVesselTypes = new List<VesselTypeDto>()
                });

            var result = await _controller.GetByCode(dockCode);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<DockDto>(okResult.Value);
            Assert.Equal(dockCode, returnValue.Code);
        }

        [Fact]
        public async Task GetByCode_ReturnsNotFound_WhenDockDoesNotExist()
        {
            var dockCode = "NONEXISTENT";
            _dockServiceMock.Setup(service => service.GetByCode(It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Dock not found"));

            var result = await _controller.GetByCode(dockCode);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetByCode_ReturnsInternalServerError_OnException()
        {
            var dockCode = "DCK001";
            _dockServiceMock.Setup(service => service.GetByCode(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.GetByCode(dockCode);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500,  objectResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenDockIsCreated()
        {
            var newDock = new CreateDockDto
            {
                Code = "DCK002",
                Name = "Secondary Dock",
                Location = "Harbor B",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            var expectedDock = new DockDto
            {
                Code = "DCK002",
                Name = "Secondary Dock",
                Location = "Harbor B",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<VesselTypeDto>()
            };

            _dockServiceMock.Setup(service => service.Add(It.IsAny<CreateDockDto>()))
                .ReturnsAsync(expectedDock);

            var result = await _controller.Create(newDock);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<DockDto>(createdAtActionResult.Value);
            Assert.Equal(newDock.Name, returnValue.Name);
        }

        [Fact]
        public async Task Create_ReturnsNotFound_WhenRelatedEntityDoesNotExist()
        {
            var newDock = new CreateDockDto
            {
                Code = "DCK002",
                Name = "Secondary Dock",
                Location = "Harbor B",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            _dockServiceMock.Setup(service => service.Add(It.IsAny<CreateDockDto>()))
                .ThrowsAsync(new EntityNotFoundException("Related entity not found"));

            var result = await _controller.Create(newDock);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenCreationFails()
        {
            var newDock = new CreateDockDto
            {
                Code = "DCK003",
                Name = "Tertiary Dock",
                Location = "Harbor C",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            _dockServiceMock.Setup(service => service.Add(It.IsAny<CreateDockDto>()))
                .ThrowsAsync(new ArgumentException("Could not create dock"));

            var result = await _controller.Create(newDock);

            var statusCodeResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenEntityAlreadyExists()
        {
            var newDock = new CreateDockDto
            {
                Code = "DCK003",
                Name = "Tertiary Dock",
                Location = "Harbor C",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            _dockServiceMock.Setup(service => service.Add(It.IsAny<CreateDockDto>()))
                .ThrowsAsync(new EntityAlreadyExistsException("This dock already exists."));

            var result = await _controller.Create(newDock);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsInternalServerError_OnException()
        {
            var newDock = new CreateDockDto
            {
                Code = "DCK004",
                Name = "Quaternary Dock",
                Location = "Harbor D",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            _dockServiceMock.Setup(service => service.Add(It.IsAny<CreateDockDto>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Create(newDock);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500,  objectResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenDockIsUpdated()
        {
            var updatedDock = new CreateDockDto
            {
                Code = "DCK001",
                Name = "Updated Dock",
                Location = "Updated Harbor",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            var expectedDock = new DockDto
            {
                Code = "DCK001",
                Name = "Updated Dock",
                Location = "Updated Harbor",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<VesselTypeDto>()
            };

            _dockServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateDockDto>()))
                .ReturnsAsync(expectedDock);

            var result = await _controller.Update("Updated Dock", updatedDock);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<DockDto>(okResult.Value);
            Assert.Equal(updatedDock.Name, returnValue.Name);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenDockDoesNotExist()
        {
            var updatedDock = new CreateDockDto
            {
                Code = "NONEXISTENT",
                Name = "Updated Dock",
                Location = "Updated Harbor",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            _dockServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateDockDto>()))
                .ThrowsAsync(new EntityNotFoundException("Dock not found"));

            var result = await _controller.Update("NONEXISTENT", updatedDock);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenInvalidDataIsProvided()
        {
            var updatedDock = new CreateDockDto
            {
                Code = "",
                Name = "",
                Location = "Updated Harbor",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            _dockServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateDockDto>()))
                .ThrowsAsync(new ArgumentException("Could not update dock"));

            var result = await _controller.Update("DCK001", updatedDock);

            var statusCodeResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_ReturnsInternalServerError_OnException()
        {
            var updatedDock = new CreateDockDto
            {
                Code = "DCK001",
                Name = "Updated Dock",
                Location = "Updated Harbor",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<string>()
            };

            _dockServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateDockDto>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Update("DCK001", updatedDock);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500,  objectResult.StatusCode);
        }

        [Fact]
        public async Task Filter_ReturnsOkResult_WithFilteredDocks()
        {
            var filter = new DockFilter
            {
                PageNumber = 1,
                PageSize = 10,
            };
            _dockServiceMock.Setup(service => service.FilterDocks(It.IsAny<DockFilter>()))
                .ReturnsAsync(new Page<DockDto> { Items = new List<DockDto>(), PageNumber = 1, PageSize = 10 });

            var result = await _controller.Filter(filter);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Page<DockDto>>(okResult.Value);
            Assert.Empty(returnValue.Items);
        }

        [Fact]
        public async Task Filter_ReturnsInternalServerError_OnException()
        {
            var filter = new DockFilter
            {
                PageNumber = 1,
                PageSize = 10,
            };
            _dockServiceMock.Setup(service => service.FilterDocks(It.IsAny<DockFilter>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Filter(filter);

            var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        }

    }
}