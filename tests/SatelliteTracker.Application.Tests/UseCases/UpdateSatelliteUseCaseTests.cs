using FluentAssertions;
using Moq;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.Tests.UseCases;

public class UpdateSatelliteUseCaseTests
{
    private readonly Mock<ISatelliteRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly UpdateSatelliteUseCase _useCase;

    public UpdateSatelliteUseCaseTests()
    {
        _mockRepo = new Mock<ISatelliteRepository>();
        _mockUow = new Mock<IUnitOfWork>();
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _useCase = new UpdateSatelliteUseCase(_mockRepo.Object, _mockUow.Object);
    }

    [Fact]
    public async Task ExecuteAsync_SatelliteExists_UpdatesAndReturnsDto()
    {
        // Arrange
        var satellite = Satellite.Create("ISS", 25544, @operator: "NASA");
        _mockRepo.Setup(r => r.GetByIdAsync(satellite.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(satellite);

        var request = new UpdateSatelliteRequest(Name: "ISS (Zarya)", Operator: "Roscosmos/NASA", Status: "Active");

        // Act
        var result = await _useCase.ExecuteAsync(satellite.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("ISS (Zarya)");
        result.Operator.Should().Be("Roscosmos/NASA");
        result.Status.Should().Be("Active");

        _mockRepo.Verify(r => r.Update(It.IsAny<Satellite>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var satellite = Satellite.Create("ISS", 25544, @operator: "NASA");
        _mockRepo.Setup(r => r.GetByIdAsync(satellite.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(satellite);

        var request = new UpdateSatelliteRequest(Name: "ISS v2");

        // Act
        var result = await _useCase.ExecuteAsync(satellite.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("ISS v2");
        result.Operator.Should().Be("NASA"); // unchanged
    }

    [Fact]
    public async Task ExecuteAsync_SatelliteNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Satellite?)null);

        var request = new UpdateSatelliteRequest(Name: "New Name");

        // Act
        var result = await _useCase.ExecuteAsync(id, request);

        // Assert
        result.Should().BeNull();
        _mockRepo.Verify(r => r.Update(It.IsAny<Satellite>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_StatusUpdate_ChangesStatus()
    {
        // Arrange
        var satellite = Satellite.Create("ISS", 25544);
        _mockRepo.Setup(r => r.GetByIdAsync(satellite.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(satellite);

        var request = new UpdateSatelliteRequest(Status: "Inactive");

        // Act
        var result = await _useCase.ExecuteAsync(satellite.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be("Inactive");
    }
}
