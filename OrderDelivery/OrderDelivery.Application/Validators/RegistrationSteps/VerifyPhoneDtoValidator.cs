using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class VerifyPhoneDtoValidator : AbstractValidator<VerifyPhoneDto>
{
    public VerifyPhoneDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.VerificationCode)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be 6 digits.")
            .Matches("^[0-9]{6}$").WithMessage("Verification code must be numeric.");
    }
} 