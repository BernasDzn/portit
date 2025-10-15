using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unitary.Service;

public class QualificationServiceTest
{
    private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
    private readonly QualificationService _service;

    private static ICollection<Qualification> qualifications = new List<Qualification>
    {
        new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }),
        new Qualification(Guid.NewGuid(), new Code { Value = "Q2" }, new Designation { Value = "Qualification 2" })
    };

    public QualificationServiceTest()
    {
        _qualificationRepositoryMock = new Mock<IQualificationRepository>();
        _service = new QualificationService(_qualificationRepositoryMock.Object, new Mock<ILogger<QualificationService>>().Object);
    }

    [Fact]
    public async Task GetQualifications_ReturnsListOfQualifications()
    {
        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationsAsync())
            .ReturnsAsync(qualifications);

        var result = await _service.GetQualifications();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(qualifications.ElementAt(0).ToDTO().IdCode, result.ElementAt(0).IdCode);
    }

    [Fact]
    public async Task GetQualificationById_ReturnsQualification_WhenExists()
    {
        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => qualifications.FirstOrDefault(q => q.NameCode.Value == id));

        var result = await _service.GetQualificationById(qualifications.ElementAt(0).NameCode.Value);

        Assert.NotNull(result);
        Assert.Equal(qualifications.ElementAt(0).ToDTO().IdCode, result.IdCode);
    }

    [Fact]
    public async Task GetQualificationById_ReturnsNull_WhenNotExists()
    {
        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Qualification?)null);

        var result = await _service.GetQualificationById("NonExistentId");

        Assert.Null(result);
    }

    [Fact]
    public async Task Add_ReturnsAddedQualification()
    {
        var newQualificationDto = new Api.Application.DataTransfer.QualificationDto
        {
            IdCode = "Q3",
            QualificationName = "Qualification 3"
        };

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(newQualificationDto.IdCode))
            .ReturnsAsync(qualifications.FirstOrDefault(q => q.NameCode.Value == newQualificationDto.IdCode));

        _qualificationRepositoryMock.Setup(repo => repo.Add(It.IsAny<Qualification>()))
            .ReturnsAsync((Qualification q) => q);

        var result = await _service.Add(newQualificationDto);

        Assert.NotNull(result);
        Assert.Equal(newQualificationDto.IdCode, result.IdCode);
    }

    [Fact]
    public async Task Add_ThrowsException_WhenQualificationAlreadyExists()
    {
        var existingQualificationDto = new Api.Application.DataTransfer.QualificationDto
        {
            IdCode = qualifications.ElementAt(0).NameCode.Value,
            QualificationName = qualifications.ElementAt(0).QualificationName.Value
        };

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(existingQualificationDto.IdCode))
            .ReturnsAsync(qualifications.FirstOrDefault(q => q.NameCode.Value == existingQualificationDto.IdCode));

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.Add(existingQualificationDto));
    }

    [Fact]
    public async Task Update_ReturnsUpdatedQualification_WhenExists()
    {
        var existingQualification = qualifications.ElementAt(0);
        var updatedQualificationDto = new Api.Application.DataTransfer.QualificationDto
        {
            IdCode = existingQualification.NameCode.Value,
            QualificationName = "Updated Qualification Name"
        };

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(existingQualification.NameCode.Value))
            .ReturnsAsync(existingQualification);

        _qualificationRepositoryMock.Setup(repo => repo.Update(It.IsAny<Qualification>()))
            .ReturnsAsync((Qualification q) => q);

        var result = await _service.Update(existingQualification.NameCode.Value, updatedQualificationDto);

        Assert.NotNull(result);
        Assert.Equal(updatedQualificationDto.QualificationName, result.QualificationName);
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenQualificationNotExists()
    {
        var nonExistentQualificationDto = new Api.Application.DataTransfer.QualificationDto
        {
            IdCode = "NonExistentId",
            QualificationName = "Some Name"
        };

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(nonExistentQualificationDto.IdCode))
            .ReturnsAsync((Qualification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Update(nonExistentQualificationDto.IdCode, nonExistentQualificationDto));
    }

    [Fact]
    public async Task FilteredQualifications_ReturnsPage()
    {
        var filter = new QualificationFilter { };

        _qualificationRepositoryMock.Setup(repo => repo.FilterQualificationsAsync(filter))
            .ReturnsAsync(new Page<Qualification>
            {
                Items = qualifications.ToList(),
                PageNumber = 1,
                PageSize = qualifications.Count
            });

        var result = await _service.FilterQualifications(filter);

        Assert.NotNull(result);
        Assert.IsType<Page<QualificationDto>>(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(qualifications.ElementAt(0).ToDTO().IdCode, result.Items.ElementAt(0).IdCode);
        Assert.Equal(qualifications.ElementAt(1).ToDTO().IdCode, result.Items.ElementAt(1).IdCode);
    }
}