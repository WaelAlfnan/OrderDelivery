using FluentValidation;
using Microsoft.AspNetCore.Http;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Constants;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class VehicleInfoDtoValidator : AbstractValidator<VehicleInfoDto>
{
    public VehicleInfoDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationPatterns.PhoneNumber).WithMessage("Invalid phone number format.");

        RuleFor(x => x.VehicleBrand)
            .NotEmpty().WithMessage("Vehicle brand is required.");
        RuleFor(x => x.VehiclePlateNumber)
            .Matches(@"^(?=.*[أ-ي]).*$").WithMessage("The vehicle plate number must contain at least one Arabic letter.")
            .Matches(@"^(?=.*[0-9]).*$").WithMessage("The vehicle plate number must contain at least one digit.")
            .Matches(@"^.{1,6}$").WithMessage("The total number of letters and digits must not exceed 6.");

        RuleFor(x => x.VehicleIssueDate)
            .NotEmpty().WithMessage("Vehicle Issue Date is required.")
            .Must(BeValidYear).WithMessage("Invalid year. The year must be between 2000 and the current year.")
            .Matches(@"^[0-9]{4}$").WithMessage("The year must be 4 digits.");

        RuleFor(x => x.ShasehPhoto)
            .NotNull().WithMessage("Shaseh photo is required.")
            .Must(BeValidImageFile).WithMessage("Shaseh photo must be a valid image file (JPG, JPEG, PNG).");
        RuleFor(x => x.EfragPhoto)
            .NotNull().WithMessage("Efrag photo is required.")
            .Must(BeValidImageFile).WithMessage("Efrag photo must be a valid image file (JPG, JPEG, PNG).");
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
    private static bool BeValidYear(string yearString)
    {
        if (int.TryParse(yearString, out int year))
        {
            return year >= 2000 && year <= DateTime.UtcNow.Year;
        }
        return false;
    }
}