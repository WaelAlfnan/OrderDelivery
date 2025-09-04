using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Constants;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class SetRoleDtoValidator : AbstractValidator<SetRoleDto>
{
    public SetRoleDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationPatterns.PhoneNumber).WithMessage("Invalid phone number format.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => r == "Merchant" || r == "Driver")
            .WithMessage("Role must be either 'Merchant' or 'Driver'.");
    }
}