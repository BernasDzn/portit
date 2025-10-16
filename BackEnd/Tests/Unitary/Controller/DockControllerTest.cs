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
                .ReturnsAsync((DockDto?)null);

            var result = await _controller.GetByCode(dockCode);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenDockIsCreated()
        {
            var newDock = new DockDto
            {
                Code = "DCK002",
                Name = "Secondary Dock",
                Location = "Harbor B",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<VesselTypeDto>()
            };

            _dockServiceMock.Setup(service => service.Add(It.IsAny<DockDto>()))
                .ReturnsAsync(newDock);

            var result = await _controller.Create(newDock);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<DockDto>(createdAtActionResult.Value);
            Assert.Equal(newDock.Name, returnValue.Name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenCreationFails()
        {
            var newDock = new DockDto
            {
                Code = "DCK003",
                Name = "Tertiary Dock",
                Location = "Harbor C",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<VesselTypeDto>()
            };

            _dockServiceMock.Setup(service => service.Add(It.IsAny<DockDto>()))
                .ReturnsAsync((DockDto?)null);

            var result = await _controller.Create(newDock);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Unable to create dock", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenDockIsUpdated()
        {
            var updatedDock = new DockDto
            {
                Code = "DCK001",
                Name = "Updated Dock",
                Location = "Updated Harbor",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<VesselTypeDto>()
            };

            _dockServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<DockDto>()))
                .ReturnsAsync(updatedDock);

            var result = await _controller.Update("Updated Dock", updatedDock);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<DockDto>(okResult.Value);
            Assert.Equal(updatedDock.Name, returnValue.Name);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenUpdateFails()
        {
            var updatedDock = new DockDto
            {
                Code = "DCK001",
                Name = "Updated Dock",
                Location = "Updated Harbor",
                PhysicalCharacteristics = null!,
                SupportedVesselTypes = new List<VesselTypeDto>()
            };

            _dockServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<DockDto>()))
                .ReturnsAsync((DockDto?)null);

            var result = await _controller.Update("DCK001", updatedDock);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Could not update dock", badRequestResult.Value);
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
        public async Task Filter_ReturnsNotFound_OnException()
        {
            var filter = new DockFilter
            {
                PageNumber = 1,
                PageSize = 10,
            };
            _dockServiceMock.Setup(service => service.FilterDocks(It.IsAny<DockFilter>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Filter(filter);

            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

    }
}