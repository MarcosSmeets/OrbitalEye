using FluentAssertions;
using Moq;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.Tests.UseCases;

public class GetTelemetryHistoryUseCaseTests
{
    private readonly Mock<ITelemetryRepository> _mockRepo;
    private readonly GetTelemetryHistoryUseCase _useCase;

    public GetTelemetryHistoryUseCaseTests()
    {
        _mockRepo = new Mock<ITelemetryRepository>();
        _useCase = new GetTelemetryHistoryUseCase(_mockRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_TelemetryExists_ReturnsListOfDtos()
    {
        // Arrange
        var satelliteId = Guid.NewGuid();
        var records = new List<Telemetry>
        {
            Telemetry.Create(satelliteId, DateTime.UtcNow.AddMinutes(-2), 51.6, -0.1, 408.0, 7.66),
            Telemetry.Create(satelliteId, DateTime.UtcNow.AddMinutes(-1), 51.7, -0.2, 407.5, 7.65),
            Telemetry.Create(satelliteId, DateTime.UtcNow, 51.8, -0.3, 407.0, 7.64)
        };

        _mockRepo.Setup(r => r.GetBySatelliteIdAsync(
            satelliteId, null, null, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(records);

        // Act
        var result = await _useCase.ExecuteAsync(satelliteId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(t => t.SatelliteId.Should().Be(satelliteId));
    }

    [Fact]
    public async Task ExecuteAsync_NoTelemetry_ReturnsEmptyList()
    {
        // Arrange
        var satelliteId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetBySatelliteIdAsync(
            satelliteId, null, null, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Telemetry>());

        // Act
        var result = await _useCase.ExecuteAsync(satelliteId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WithDateRange_PassesParametersToRepository()
    {
        // Arrange
        var satelliteId = Guid.NewGuid();
        var from = DateTime.UtcNow.AddHours(-1);
        var to = DateTime.UtcNow;
        var limit = 50;

        _mockRepo.Setup(r => r.GetBySatelliteIdAsync(
            satelliteId, from, to, limit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Telemetry>());

        // Act
        await _useCase.ExecuteAsync(satelliteId, from, to, limit);

        // Assert
        _mockRepo.Verify(r => r.GetBySatelliteIdAsync(
            satelliteId, from, to, limit, It.IsAny<CancellationToken>()), Times.Once);
    }
}
