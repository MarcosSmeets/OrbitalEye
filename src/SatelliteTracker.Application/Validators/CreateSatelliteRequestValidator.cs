using FluentValidation;
using SatelliteTracker.Application.DTOs;

namespace SatelliteTracker.Application.Validators;

public class CreateSatelliteRequestValidator : AbstractValidator<CreateSatelliteRequest>
{
    public CreateSatelliteRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.NoradId).GreaterThan(0).WithMessage("NORAD ID must be greater than zero.");
    }
}
