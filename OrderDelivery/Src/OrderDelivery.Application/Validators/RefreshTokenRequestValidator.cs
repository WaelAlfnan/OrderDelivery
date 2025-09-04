using FluentValidation;
using OrderDelivery.Application.DTOs.Requests;

namespace OrderDelivery.Application.Validators
{
    /// <summary>
    /// Validator for RefreshTokenRequest
    /// </summary>
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.")
                .MaximumLength(1000)
                .WithMessage("Refresh token cannot exceed 1000 characters.");
        }
    }
}
