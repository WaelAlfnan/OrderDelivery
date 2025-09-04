using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class UserMapperTests
{
    [Fact]
    public void ToUserDto_ValidUser_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var role = "Driver";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("أحمد علي", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.True(result.IsPhoneConfirmed);
        Assert.Equal("Driver", result.Role);
    }

    [Fact]
    public void ToUserDto_UserWithEmptyNames_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "",
            LastName = "",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = false
        };
        var role = "Merchant";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.False(result.IsPhoneConfirmed);
        Assert.Equal("Merchant", result.Role);
    }

    [Fact]
    public void ToUserDto_UserWithNullNames_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = string.Empty,
            LastName = string.Empty,
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var role = "Admin";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.True(result.IsPhoneConfirmed);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public void ToUserDto_UserWithNullPhoneNumber_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = null,
            PhoneNumberConfirmed = false
        };
        var role = "Driver";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("", result.PhoneNumber);
        Assert.False(result.IsPhoneConfirmed);
        Assert.Equal("Driver", result.Role);
    }

    [Fact]
    public void ToUserDto_UserWithOnlyFirstName_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var role = "Driver";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("أحمد", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.True(result.IsPhoneConfirmed);
        Assert.Equal("Driver", result.Role);
    }

    [Fact]
    public void ToUserDto_UserWithOnlyLastName_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "",
            LastName = "علي",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var role = "Driver";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("علي", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.True(result.IsPhoneConfirmed);
        Assert.Equal("Driver", result.Role);
    }

    [Fact]
    public void ToUserDto_UserWithWhitespaceNames_ReturnsTrimmedUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "  أحمد  ",
            LastName = "  علي  ",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var role = "Driver";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("أحمد علي", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.True(result.IsPhoneConfirmed);
        Assert.Equal("Driver", result.Role);
    }

    [Fact]
    public void ToUserDto_NullUser_ThrowsArgumentNullException()
    {
        // Arrange
        ApplicationUser? user = null;
        var role = "Driver";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => user!.ToUserDto(role));
        Assert.Equal("user", exception.ParamName);
    }

    [Fact]
    public void ToUserDto_UserWithSpecialCharacters_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "محمد-أحمد",
            LastName = "علي_الحسن",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var role = "Driver";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("محمد-أحمد علي_الحسن", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.True(result.IsPhoneConfirmed);
        Assert.Equal("Driver", result.Role);
    }

    [Fact]
    public void ToUserDto_UserWithEmptyRole_ReturnsCorrectUserDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var role = "";

        // Act
        var result = user.ToUserDto(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("أحمد علي", result.FullName);
        Assert.Equal("+966501234567", result.PhoneNumber);
        Assert.True(result.IsPhoneConfirmed);
        Assert.Equal("", result.Role);
    }
}
