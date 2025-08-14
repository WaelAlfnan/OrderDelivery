using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.Mappers;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace UnitTests.Mappers;

public class RegisterDtoMapperTests
{
    [Fact]
    public void ToApplicationUser_ValidDto_ReturnsCorrectEntity()
    {
        // Arrange
        var dto = new RegisterDto("محمد أحمد", "0500000000", "pass123", "User");
        // Act
        var entity = dto.ToApplicationUser();
        // Assert
        Assert.Equal(dto.PhoneNumber, entity.UserName);
        Assert.Equal(dto.PhoneNumber, entity.PhoneNumber);
        Assert.Equal("محمد", entity.FirstName);
        Assert.Equal("أحمد", entity.LastName);
        Assert.False(entity.PhoneNumberConfirmed);
        Assert.False(entity.EmailConfirmed);
        Assert.False(entity.TwoFactorEnabled);
        Assert.False(entity.LockoutEnabled);
    }

    [Fact]
    public void ToApplicationUser_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        RegisterDto? dto = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RegisterDtoMapper.ToApplicationUser(dto!));
    }

    [Fact]
    public void ToApplicationUser_OnlyFirstName_SetsLastNameEmpty()
    {
        // Arrange
        var dto = new RegisterDto("محمد", "0500000000", "pass123", "User");
        // Act
        var entity = dto.ToApplicationUser();
        // Assert
        Assert.Equal("محمد", entity.FirstName);
        Assert.Equal(string.Empty, entity.LastName);
    }
} 