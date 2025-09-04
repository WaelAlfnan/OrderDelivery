using FluentValidation;
using OrderDelivery.Application.DTOs.Requests;

namespace OrderDelivery.Application.Validators;

public class PasswordResetSessionDtoValidator : AbstractValidator<PasswordResetSessionDto>
{
    public PasswordResetSessionDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .NotNull().WithMessage("Phone number cannot be null")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format. Must be between 10-15 digits")
            .MaximumLength(20).WithMessage("Phone number is too long")
            .Must(phone => !string.IsNullOrWhiteSpace(phone)).WithMessage("Phone number cannot contain only whitespace");
        
        RuleFor(x => x.SessionToken)
            .NotEmpty().WithMessage("Session token is required")
            .NotNull().WithMessage("Session token cannot be null")
            .MinimumLength(32).WithMessage("Session token is too short")
            .MaximumLength(128).WithMessage("Session token is too long");
    }
}
