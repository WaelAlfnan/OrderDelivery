using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class MerchantInfoDtoValidator : AbstractValidator<MerchantInfoDto>
{
    public MerchantInfoDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Store name is required.");
        RuleFor(x => x.StoreType)
            .NotEmpty().WithMessage("Store type is required.");
        RuleFor(x => x.StoreAddress)
            .NotEmpty().WithMessage("Store address is required.");
        RuleFor(x => x.StoreLatitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.StoreLongitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        // BusinessLicenseNumber is optional
    }
} 