using FluentValidation;
using OrderDelivery.Application.DTOs.Requests;

namespace OrderDelivery.Application.Validators;

public class SetNewPasswordDtoValidator : AbstractValidator<SetNewPasswordDto>
{
    public SetNewPasswordDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .NotNull().WithMessage("Phone number cannot be null")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format. Must be between 10-15 digits")
            .MaximumLength(20).WithMessage("Phone number is too long")
            .Must(phone => !string.IsNullOrWhiteSpace(phone)).WithMessage("Phone number cannot contain only whitespace");
        
        RuleFor(x => x.VerificationCode)
            .NotEmpty().WithMessage("Verification code is required")
            .NotNull().WithMessage("Verification code cannot be null")
            .Length(6).WithMessage("Verification code must be exactly 6 digits")
            .Matches(@"^[0-9]{6}$").WithMessage("Verification code must contain only numbers")
            .Must(code => !string.IsNullOrWhiteSpace(code)).WithMessage("Verification code cannot contain only whitespace");
        
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .NotNull().WithMessage("New password cannot be null")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(128).WithMessage("Password is too long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")
            .Must(password => !string.IsNullOrWhiteSpace(password)).WithMessage("Password cannot contain only whitespace");
        
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required")
            .NotNull().WithMessage("Password confirmation cannot be null")
            .Equal(x => x.NewPassword).WithMessage("Password and confirmation do not match")
            .Must(confirmPassword => !string.IsNullOrWhiteSpace(confirmPassword)).WithMessage("Password confirmation cannot contain only whitespace");
        
        RuleFor(x => x.SessionToken)
            .NotEmpty().WithMessage("Session token is required")
            .NotNull().WithMessage("Session token cannot be null")
            .MinimumLength(32).WithMessage("Session token is too short")
            .MaximumLength(128).WithMessage("Session token is too long");
    }
}
