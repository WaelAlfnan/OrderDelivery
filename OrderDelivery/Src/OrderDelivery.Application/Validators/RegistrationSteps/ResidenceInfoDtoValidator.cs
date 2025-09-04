using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Constants;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class ResidenceInfoDtoValidator : AbstractValidator<ResidenceInfoDto>
{
    public ResidenceInfoDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationPatterns.PhoneNumber).WithMessage("Invalid phone number format.");

        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Province is required.");
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.District)
            .NotEmpty().WithMessage("District is required.");
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.");
        RuleFor(x => x.BuildingNumber)
            .NotEmpty().WithMessage("Building number is required.");
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}