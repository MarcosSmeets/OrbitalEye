using FluentAssertions;
using SatelliteTracker.Domain.ValueObjects;

namespace SatelliteTracker.Domain.Tests.ValueObjects;

public class VelocityTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateVelocity()
    {
        var velocity = new Velocity(7.8, 45.0);

        velocity.Magnitude.Should().Be(7.8);
        velocity.Direction.Should().Be(45.0);
    }

    [Fact]
    public void Constructor_ZeroMagnitude_ShouldCreateVelocity()
    {
        var velocity = new Velocity(0.0, 180.0);

        velocity.Magnitude.Should().Be(0.0);
        velocity.Direction.Should().Be(180.0);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.001)]
    [InlineData(-100)]
    public void Constructor_NegativeMagnitude_ShouldThrowArgumentOutOfRangeException(double magnitude)
    {
        Action act = () => new Velocity(magnitude, 0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("magnitude");
    }

    [Fact]
    public void Constructor_NegativeDirection_ShouldCreateVelocity()
    {
        var velocity = new Velocity(5.0, -45.0);

        velocity.Direction.Should().Be(-45.0);
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        var velocity1 = new Velocity(7.8, 45.0);
        var velocity2 = new Velocity(7.8, 45.0);

        velocity1.Should().Be(velocity2);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        var velocity1 = new Velocity(7.8, 45.0);
        var velocity2 = new Velocity(7.8, 90.0);

        velocity1.Should().NotBe(velocity2);
    }
}
