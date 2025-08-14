using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class RegisterPhoneDtoValidator : AbstractValidator<RegisterPhoneDto>
{
    public RegisterPhoneDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number format.");
    }
} 