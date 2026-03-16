using FluentValidation;
using SatelliteTracker.Application.DTOs;

namespace SatelliteTracker.Application.Validators;

public class CreateGroundStationRequestValidator : AbstractValidator<CreateGroundStationRequest>
{
    public CreateGroundStationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required.");
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(x => x.Altitude).GreaterThanOrEqualTo(0).WithMessage("Altitude must be greater than or equal to zero.");
    }
}
