using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.Validators.RegistrationSteps;
using Xunit;

namespace OrderDelivery.UnitTests.Validators;

public class SetBasicInfoDtoValidatorTests
{
    private readonly SetBasicInfoDtoValidator _validator;

    public SetBasicInfoDtoValidatorTests()
    {
        _validator = new SetBasicInfoDtoValidator();
    }

    [Fact]
    public void Validate_ValidSetBasicInfoDto_ShouldNotHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPhoneNumber_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number is required.");
    }

    [Fact]
    public void Validate_InvalidPhoneNumberFormat_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "0501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Fact]
    public void Validate_EmptyFullName_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name is required.");
    }

    [Fact]
    public void Validate_NullPersonalPhoto_ShouldHaveValidationError()
    {
        // Arrange
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: null!,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PersonalPhoto)
            .WithErrorMessage("Personal photo is required.");
    }

    [Fact]
    public void Validate_InvalidPersonalPhotoExtension_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.txt", "text/plain", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PersonalPhoto)
            .WithErrorMessage("Personal photo must be a valid image file (JPG, JPEG, PNG).");
    }

    [Fact]
    public void Validate_PersonalPhotoTooLarge_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 6 * 1024 * 1024); // 6MB
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PersonalPhoto)
            .WithErrorMessage("Personal photo must be a valid image file (JPG, JPEG, PNG).");
    }

    [Fact]
    public void Validate_EmptyNationalIdNumber_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: ""
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.NationalIdNumber)
            .WithErrorMessage("National ID number is required.");
    }

    [Fact]
    public void Validate_NonNumericNationalIdNumber_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "1234567890123a"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.NationalIdNumber)
            .WithErrorMessage("National ID number must be numeric.");
    }

    [Fact]
    public void Validate_NationalIdNumberTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "1234567890123"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.NationalIdNumber)
            .WithErrorMessage("National ID number must be exactly 14 digits.");
    }

    [Fact]
    public void Validate_NationalIdNumberTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "123456789012345"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.NationalIdNumber)
            .WithErrorMessage("National ID number must be exactly 14 digits.");
    }

    [Fact]
    public void Validate_ValidPngFile_ShouldNotHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.png", "image/png", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.png", "image/png", 1024);
        var mockBackPhoto = CreateMockFormFile("back.png", "image/png", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidJpegFile_ShouldNotHaveValidationError()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal.jpeg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpeg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpeg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: "+966501234567",
            FullName: "أحمد علي",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "12345678901234"
        );

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static IFormFile CreateMockFormFile(string fileName, string contentType, long length)
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        return mockFile.Object;
    }
}
