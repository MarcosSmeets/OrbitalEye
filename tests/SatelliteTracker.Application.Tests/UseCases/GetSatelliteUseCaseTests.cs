using FluentAssertions;
using Moq;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.Tests.UseCases;

public class GetSatelliteUseCaseTests
{
    private readonly Mock<ISatelliteRepository> _mockRepo;
    private readonly GetSatelliteUseCase _useCase;

    public GetSatelliteUseCaseTests()
    {
        _mockRepo = new Mock<ISatelliteRepository>();
        _useCase = new GetSatelliteUseCase(_mockRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_SatelliteExists_ReturnsDto()
    {
        // Arrange
        var satellite = Satellite.Create("ISS", 25544, "1998-067A", "NASA");
        _mockRepo.Setup(r => r.GetByIdAsync(satellite.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(satellite);

        // Act
        var result = await _useCase.ExecuteAsync(satellite.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("ISS");
        result.NoradId.Should().Be(25544);
        result.Id.Should().Be(satellite.Id);
    }

    [Fact]
    public async Task ExecuteAsync_SatelliteNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Satellite?)null);

        // Act
        var result = await _useCase.ExecuteAsync(id);

        // Assert
        result.Should().BeNull();
    }
}
