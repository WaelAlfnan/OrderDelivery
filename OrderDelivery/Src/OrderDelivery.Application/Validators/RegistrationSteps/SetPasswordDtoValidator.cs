using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Constants;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class SetPasswordDtoValidator : AbstractValidator<SetPasswordDto>
{
    public SetPasswordDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationPatterns.PhoneNumber).WithMessage("Invalid phone number format.");


        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[!@#$%^&*(),.?\\\":{}|<>_-]").WithMessage("Password must contain at least one special character.");
    }
}
