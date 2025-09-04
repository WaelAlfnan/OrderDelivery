using FluentValidation;
using Microsoft.AspNetCore.Http;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Constants;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class SetBasicInfoDtoValidator : AbstractValidator<SetBasicInfoDto>
{
    public SetBasicInfoDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationPatterns.PhoneNumber).WithMessage("Invalid phone number format.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.");

        RuleFor(x => x.PersonalPhoto)
            .NotNull().WithMessage("Personal photo is required.")
            .Must(BeValidImageFile).WithMessage("Personal photo must be a valid image file (JPG, JPEG, PNG).");

        RuleFor(x => x.NationalIdFrontPhoto)
            .NotNull().WithMessage("National ID front photo is required.")
            .Must(BeValidImageFile).WithMessage("National ID front photo must be a valid image file (JPG, JPEG, PNG).");

        RuleFor(x => x.NationalIdBackPhoto)
            .NotNull().WithMessage("National ID back photo is required.")
            .Must(BeValidImageFile).WithMessage("National ID back photo must be a valid image file (JPG, JPEG, PNG).");

        RuleFor(x => x.NationalIdNumber)
            .NotEmpty().WithMessage("National ID number is required.")
            .Matches("^[0-9]+$").WithMessage("National ID number must be numeric.")
            .Matches("^[0-9]{14}$").WithMessage("National ID number must be exactly 14 digits.");
    }

    private static bool BeValidImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false; // Photos are now mandatory

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(fileExtension) &&
               file.Length <= 5 * 1024 * 1024; // Max 5MB
    }
}
