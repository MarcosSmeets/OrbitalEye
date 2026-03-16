using FluentAssertions;
using FluentValidation;
using Moq;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Interfaces;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.Tests.UseCases;

public class IngestTelemetryUseCaseTests
{
    private readonly Mock<ITelemetryRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<ITelemetryBroadcaster> _mockBroadcaster;
    private readonly IngestTelemetryUseCase _useCase;
    private readonly IngestTelemetryUseCase _useCaseWithBroadcaster;

    public IngestTelemetryUseCaseTests()
    {
        _mockRepo = new Mock<ITelemetryRepository>();
        _mockUow = new Mock<IUnitOfWork>();
        _mockBroadcaster = new Mock<ITelemetryBroadcaster>();
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _useCase = new IngestTelemetryUseCase(_mockRepo.Object, _mockUow.Object);
        _useCaseWithBroadcaster = new IngestTelemetryUseCase(_mockRepo.Object, _mockUow.Object, _mockBroadcaster.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_IngestsTelemetryAndReturnsDto()
    {
        // Arrange
        var satelliteId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        var request = new IngestTelemetryRequest(
            satelliteId, timestamp, 51.6, -0.1, 408.0, 7.66, 20.0, 85.0, -60.0);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.SatelliteId.Should().Be(satelliteId);
        result.Latitude.Should().Be(51.6);
        result.Longitude.Should().Be(-0.1);
        result.Altitude.Should().Be(408.0);
        result.Velocity.Should().Be(7.66);
        result.Temperature.Should().Be(20.0);
        result.BatteryLevel.Should().Be(85.0);
        result.SignalStrength.Should().Be(-60.0);

        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Telemetry>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithBroadcaster_BroadcastsUpdates()
    {
        // Arrange
        var satelliteId = Guid.NewGuid();
        var request = new IngestTelemetryRequest(
            satelliteId, DateTime.UtcNow, 45.0, 90.0, 400.0, 7.5);

        // Act
        await _useCaseWithBroadcaster.ExecuteAsync(request);

        // Assert
        _mockBroadcaster.Verify(b => b.BroadcastPositionUpdateAsync(
            satelliteId, 45.0, 90.0, 400.0, It.IsAny<CancellationToken>()), Times.Once);
        _mockBroadcaster.Verify(b => b.BroadcastTelemetryUpdateAsync(
            satelliteId, It.IsAny<TelemetryDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutBroadcaster_DoesNotThrow()
    {
        // Arrange
        var request = new IngestTelemetryRequest(
            Guid.NewGuid(), DateTime.UtcNow, 0.0, 0.0, 400.0, 7.5);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteAsync_EmptySatelliteId_ThrowsValidationException()
    {
        // Arrange
        var request = new IngestTelemetryRequest(
            Guid.Empty, DateTime.UtcNow, 0.0, 0.0, 400.0, 7.5);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "SatelliteId"));
    }

    [Fact]
    public async Task ExecuteAsync_InvalidLatitude_ThrowsValidationException()
    {
        // Arrange
        var request = new IngestTelemetryRequest(
            Guid.NewGuid(), DateTime.UtcNow, 91.0, 0.0, 400.0, 7.5);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Latitude"));
    }

    [Fact]
    public async Task ExecuteAsync_InvalidLongitude_ThrowsValidationException()
    {
        // Arrange
        var request = new IngestTelemetryRequest(
            Guid.NewGuid(), DateTime.UtcNow, 0.0, 181.0, 400.0, 7.5);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Longitude"));
    }

    [Fact]
    public async Task ExecuteAsync_NegativeAltitude_ThrowsValidationException()
    {
        // Arrange
        var request = new IngestTelemetryRequest(
            Guid.NewGuid(), DateTime.UtcNow, 0.0, 0.0, -1.0, 7.5);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Altitude"));
    }

    [Fact]
    public async Task ExecuteAsync_InvalidBatteryLevel_ThrowsValidationException()
    {
        // Arrange
        var request = new IngestTelemetryRequest(
            Guid.NewGuid(), DateTime.UtcNow, 0.0, 0.0, 400.0, 7.5, BatteryLevel: 101.0);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "BatteryLevel"));
    }
}
