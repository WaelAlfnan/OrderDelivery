using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Constants;
using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class DriverInfoDtoValidator : AbstractValidator<DriverInfoDto>
{
    public DriverInfoDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationPatterns.PhoneNumber).WithMessage("Invalid phone number format.");

        RuleFor(x => x.VehicleType)
            .NotEmpty().WithMessage("Vehicle type is required.")
            .Must(vt => Enum.TryParse<VehicleType>(vt, true, out _))
            .WithMessage("Invalid vehicle type.");
        RuleFor(x => x.CurrentLatitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.CurrentLongitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}