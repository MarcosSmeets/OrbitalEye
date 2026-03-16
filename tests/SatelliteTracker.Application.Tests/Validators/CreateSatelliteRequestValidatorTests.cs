using FluentAssertions;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Validators;

namespace SatelliteTracker.Application.Tests.Validators;

public class CreateSatelliteRequestValidatorTests
{
    private readonly CreateSatelliteRequestValidator _validator;

    public CreateSatelliteRequestValidatorTests()
    {
        _validator = new CreateSatelliteRequestValidator();
    }

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        // Arrange
        var request = new CreateSatelliteRequest("ISS", 25544, "1998-067A", "NASA");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_MinimalValidRequest_Passes()
    {
        // Arrange
        var request = new CreateSatelliteRequest("Hubble", 20580);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task Validate_EmptyOrNullName_Fails(string? name)
    {
        // Arrange
        var request = new CreateSatelliteRequest(name!, 25544);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Validate_NoradIdNotGreaterThanZero_Fails(int noradId)
    {
        // Arrange
        var request = new CreateSatelliteRequest("ISS", noradId);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NoradId");
    }

    [Fact]
    public async Task Validate_BothInvalid_ReturnsMultipleErrors()
    {
        // Arrange
        var request = new CreateSatelliteRequest("", 0);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
