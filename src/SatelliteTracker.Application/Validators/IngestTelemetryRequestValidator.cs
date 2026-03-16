using FluentValidation;
using SatelliteTracker.Application.DTOs;

namespace SatelliteTracker.Application.Validators;

public class IngestTelemetryRequestValidator : AbstractValidator<IngestTelemetryRequest>
{
    public IngestTelemetryRequestValidator()
    {
        RuleFor(x => x.SatelliteId).NotEmpty().WithMessage("Satellite ID is required.");
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(x => x.Altitude).GreaterThanOrEqualTo(0).WithMessage("Altitude must be greater than or equal to zero.");
        RuleFor(x => x.Velocity).GreaterThanOrEqualTo(0).WithMessage("Velocity must be greater than or equal to zero.");
        RuleFor(x => x.BatteryLevel)
            .InclusiveBetween(0, 100)
            .When(x => x.BatteryLevel.HasValue)
            .WithMessage("Battery level must be between 0 and 100.");
    }
}
