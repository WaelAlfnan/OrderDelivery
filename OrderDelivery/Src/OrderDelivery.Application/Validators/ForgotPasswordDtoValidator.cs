using FluentValidation;
using OrderDelivery.Application.DTOs.Requests;

namespace OrderDelivery.Application.Validators;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .NotNull().WithMessage("Phone number cannot be null")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format. Must be between 10-15 digits")
            .MaximumLength(20).WithMessage("Phone number is too long")
            .Must(phone => !string.IsNullOrWhiteSpace(phone)).WithMessage("Phone number cannot contain only whitespace");
    }
} 