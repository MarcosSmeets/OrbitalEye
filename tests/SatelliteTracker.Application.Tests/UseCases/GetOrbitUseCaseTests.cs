using FluentAssertions;
using Moq;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.Tests.UseCases;

public class GetOrbitUseCaseTests
{
    private readonly Mock<IOrbitRepository> _mockRepo;
    private readonly GetOrbitUseCase _useCase;

    public GetOrbitUseCaseTests()
    {
        _mockRepo = new Mock<IOrbitRepository>();
        _useCase = new GetOrbitUseCase(_mockRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_OrbitExists_ReturnsDto()
    {
        // Arrange
        var satelliteId = Guid.NewGuid();
        var orbit = Orbit.Create(
            satelliteId,
            inclination: 51.6,
            eccentricity: 0.0001,
            rightAscension: 200.0,
            argumentOfPerigee: 100.0,
            meanAnomaly: 250.0,
            meanMotion: 15.5,
            epoch: new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        _mockRepo.Setup(r => r.GetBySatelliteIdAsync(satelliteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orbit);

        // Act
        var result = await _useCase.ExecuteAsync(satelliteId);

        // Assert
        result.Should().NotBeNull();
        result!.SatelliteId.Should().Be(satelliteId);
        result.Inclination.Should().Be(51.6);
        result.Eccentricity.Should().Be(0.0001);
        result.MeanMotion.Should().Be(15.5);
    }

    [Fact]
    public async Task ExecuteAsync_OrbitNotFound_ReturnsNull()
    {
        // Arrange
        var satelliteId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetBySatelliteIdAsync(satelliteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Orbit?)null);

        // Act
        var result = await _useCase.ExecuteAsync(satelliteId);

        // Assert
        result.Should().BeNull();
    }
}
