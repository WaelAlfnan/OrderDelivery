using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.Validators;
using Xunit;

namespace OrderDelivery.UnitTests.Validators;

public class LoginDtoValidatorTests
{
    private readonly LoginDtoValidator _validator;

    public LoginDtoValidatorTests()
    {
        _validator = new LoginDtoValidator();
    }

    [Fact]
    public void Validate_ValidLoginDto_ShouldNotHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPhoneNumber_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number is required.");
    }

    [Fact]
    public void Validate_NullPhoneNumber_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto(null!, "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number is required.");
    }

    [Fact]
    public void Validate_InvalidPhoneNumberFormat_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("0501234567", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Fact]
    public void Validate_PhoneNumberWithoutCountryCode_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("501234567", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Fact]
    public void Validate_ValidPhoneNumberWithCountryCode_ShouldNotHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_EmptyPassword_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Validate_NullPassword_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", null!);

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Validate_ValidPassword_ShouldNotHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_BothFieldsEmpty_ShouldHaveValidationErrorsForBoth()
    {
        // Arrange
        var loginDto = new LoginDto("", "");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number is required.");
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Validate_PhoneNumberWithSpecialCharacters_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("+966-501-234-567", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Fact]
    public void Validate_PhoneNumberWithSpaces_ShouldHaveValidationError()
    {
        // Arrange
        var loginDto = new LoginDto("+966 501 234 567", "password123");

        // Act & Assert
        var result = _validator.TestValidate(loginDto);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }
}
