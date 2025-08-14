using FluentValidation;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using Microsoft.AspNetCore.Http;

namespace OrderDelivery.Application.Validators.RegistrationSteps;

public class VehicleInfoDtoValidator : AbstractValidator<VehicleInfoDto>
{
    public VehicleInfoDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.VehicleBrand)
            .NotEmpty().WithMessage("Vehicle brand is required.");
        RuleFor(x => x.VehiclePlateNumber)
            .NotEmpty().WithMessage("Vehicle plate number is required.");
        RuleFor(x => x.VehicleIssueDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Vehicle issue date cannot be in the future.");
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
} 