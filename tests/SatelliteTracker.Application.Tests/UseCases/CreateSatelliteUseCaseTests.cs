using FluentAssertions;
using FluentValidation;
using Moq;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.Tests.UseCases;

public class CreateSatelliteUseCaseTests
{
    private readonly Mock<ISatelliteRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly CreateSatelliteUseCase _useCase;

    public CreateSatelliteUseCaseTests()
    {
        _mockRepo = new Mock<ISatelliteRepository>();
        _mockUow = new Mock<IUnitOfWork>();
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _useCase = new CreateSatelliteUseCase(_mockRepo.Object, _mockUow.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_CreatesSatelliteAndReturnsDto()
    {
        // Arrange
        var request = new CreateSatelliteRequest(
            "ISS", 25544, "1998-067A", "NASA", new DateTime(1998, 11, 20));

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("ISS");
        result.NoradId.Should().Be(25544);
        result.InternationalDesignator.Should().Be("1998-067A");
        result.Operator.Should().Be("NASA");
        result.Status.Should().Be("Active");
        result.Id.Should().NotBeEmpty();

        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Satellite>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_MinimalRequest_CreatesSatelliteWithDefaults()
    {
        // Arrange
        var request = new CreateSatelliteRequest("Hubble", 20580);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Hubble");
        result.NoradId.Should().Be(20580);
        result.InternationalDesignator.Should().BeNull();
        result.Operator.Should().BeNull();
        result.LaunchDate.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_EmptyName_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateSatelliteRequest("", 25544);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Name"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task ExecuteAsync_InvalidNoradId_ThrowsValidationException(int noradId)
    {
        // Arrange
        var request = new CreateSatelliteRequest("ISS", noradId);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "NoradId"));
    }

    [Fact]
    public async Task ExecuteAsync_EmptyNameAndInvalidNoradId_ThrowsValidationExceptionWithMultipleErrors()
    {
        // Arrange
        var request = new CreateSatelliteRequest("", 0);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
