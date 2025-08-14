using FluentValidation;
using OrderDelivery.Application.DTOs.Requests;

namespace OrderDelivery.Application.Validators;

public class UploadPhotoDtoValidator : AbstractValidator<UploadPhotoDto>
{
    public UploadPhotoDtoValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.")
            .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
            .Must(file => file.Length <= 10 * 1024 * 1024).WithMessage("File size cannot exceed 10MB.");

        RuleFor(x => x.File.ContentType)
            .Must(contentType => contentType.StartsWith("image/"))
            .WithMessage("Only image files are allowed.");

        RuleFor(x => x.File.FileName)
            .Must(fileName => !string.IsNullOrEmpty(fileName))
            .WithMessage("File name is required.")
            .Must(fileName => fileName.Length <= 255)
            .WithMessage("File name is too long.");
    }
}
