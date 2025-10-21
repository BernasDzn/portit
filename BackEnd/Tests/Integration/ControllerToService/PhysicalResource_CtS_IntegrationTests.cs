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
using Api.Domain.IRepository;
using Microsoft.Extensions.Logging;
using Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.ValueObjects;

namespace Tests.Integration.ControllerToService;

public class PhysicalResource_CtS_IntegrationTest
{
    private readonly PhysicalResourceController _controller;
    private readonly PhysicalResourceService _service;
    private readonly Mock<IPhysicalResourceRepository> _repositoryMock;
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock;

    public PhysicalResource_CtS_IntegrationTest()
    {
        _repositoryMock = new Mock<IPhysicalResourceRepository>();
        _dockRepositoryMock = new Mock<IDockRepository>();
        _qualificationRepositoryMock = new Mock<IQualificationRepository>();
        _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();

        _service = new PhysicalResourceService(
            _repositoryMock.Object,
            _dockRepositoryMock.Object,
            _qualificationRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            new Mock<ILogger<PhysicalResourceService>>().Object
        );
        _controller = new PhysicalResourceController(_service, new Mock<ILogger<PhysicalResourceController>>().Object);
    }
}
