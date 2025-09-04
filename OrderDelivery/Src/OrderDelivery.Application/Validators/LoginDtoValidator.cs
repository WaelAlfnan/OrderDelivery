using FluentValidation;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Domain.Constants;

namespace OrderDelivery.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationPatterns.PhoneNumber).WithMessage("Invalid phone number format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}