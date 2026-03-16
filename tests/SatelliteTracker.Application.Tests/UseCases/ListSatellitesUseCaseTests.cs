using FluentAssertions;
using Moq;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.Tests.UseCases;

public class ListSatellitesUseCaseTests
{
    private readonly Mock<ISatelliteRepository> _mockRepo;
    private readonly ListSatellitesUseCase _useCase;

    public ListSatellitesUseCaseTests()
    {
        _mockRepo = new Mock<ISatelliteRepository>();
        _useCase = new ListSatellitesUseCase(_mockRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_SatellitesExist_ReturnsListOfDtos()
    {
        // Arrange
        var satellites = new List<Satellite>
        {
            Satellite.Create("ISS", 25544),
            Satellite.Create("Hubble", 20580),
            Satellite.Create("NOAA-20", 43013)
        };
        _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(satellites);

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Select(s => s.Name).Should().Contain(new[] { "ISS", "Hubble", "NOAA-20" });
    }

    [Fact]
    public async Task ExecuteAsync_NoSatellites_ReturnsEmptyList()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Satellite>());

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
